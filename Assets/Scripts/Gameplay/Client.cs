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
    }

    private void FixedUpdate()
    {
        if (client != null)
        {
            if (lobby)
            {
                if (client.readyToListen)
                {
                    client.myInfo.colorID = colorID;
                }
            }
            else
            {
                client.myInfo.playerPos = myPlayer.transform.position;
                hostPlayer.transform.position = client.hostInfo.playerPos;
            }
        }
    }
}
