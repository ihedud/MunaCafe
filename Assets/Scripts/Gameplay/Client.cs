using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] private GameObject myPlayer;
    [SerializeField] private GameObject hostPlayer;
    [SerializeField] private bool lobby = false;
    [SerializeField] private List<Material> materials;
    private ClientUDP client;
    public int colorID;

    private void Awake()
    {
        client = FindObjectOfType<ClientUDP>();
        if (!lobby)
        {
            myPlayer.GetComponentInChildren<MeshRenderer>().material = materials[client.myPlayer.colorID];
            hostPlayer.GetComponentInChildren<MeshRenderer>().material = materials[client.hostPlayer.colorID];
        }
    }

    private void FixedUpdate()
    {
        if (client != null)
        {
            if (lobby)
            {
                if (client.readyToListen)
                {
                    client.myPlayer.colorID = colorID;
                    hostPlayer.GetComponent<MeshRenderer>().material = materials[client.hostPlayer.colorID];
                }
            }
            else
            {
                hostPlayer.transform.position = client.hostPlayer.playerPos;
                client.myPlayer.playerPos = myPlayer.transform.position;

                if (client.hostPlayer.hasPing && !client.pingDone)
                {
                    hostPlayer.GetComponent<PlayerCommunicating>().ShowPingFromMessage();
                    client.pingDone = true;
                }

                if (myPlayer.GetComponent<PlayerCommunicating>().isShowing)
                    client.myPlayer.hasPing = true;
                else
                    client.myPlayer.hasPing = false;
            }
        }
    }
}
