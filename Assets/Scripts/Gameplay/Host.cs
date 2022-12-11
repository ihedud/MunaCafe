using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    [SerializeField] private GameObject myPlayer;
    [SerializeField] private GameObject clientPlayer;
    [SerializeField] private bool lobby = false;
    [SerializeField] private List<Material> materials;
    private HostUDP host;
    public int colorID;

    private void Awake()
    {
        host = FindObjectOfType<HostUDP>();
        if (!lobby)
        {
            myPlayer.GetComponentInChildren<MeshRenderer>().material = materials[host.myPlayer.colorID];
            clientPlayer.GetComponentInChildren<MeshRenderer>().material = materials[host.clientPlayer.colorID];
        }
    }

    private void FixedUpdate()
    {
        if (host != null)
        {
            if (lobby)
            {
                if (host.readyToListen)
                {
                    host.myPlayer.colorID = colorID;
                    clientPlayer.GetComponent<MeshRenderer>().material = materials[host.clientPlayer.colorID];
                }
            }
            else
            {
                clientPlayer.transform.position = host.clientPlayer.playerPos;
                host.myPlayer.playerPos = myPlayer.transform.position;

                if (host.clientPlayer.hasPing && !host.pingDone)
                {
                    clientPlayer.GetComponent<PlayerCommunicating>().ShowPingFromMessage();
                    host.pingDone = true;
                }

                if (myPlayer.GetComponent<PlayerCommunicating>().isShowing)
                    host.myPlayer.hasPing = true;
                else
                    host.myPlayer.hasPing = false;
            }
        }
    }
}
