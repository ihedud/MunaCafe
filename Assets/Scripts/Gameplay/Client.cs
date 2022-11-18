using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] private GameObject myPlayer;
    [SerializeField] private GameObject hostPlayer;
    private ClientUDP client;

    private void Awake()
    {
        client = FindObjectOfType<ClientUDP>();
    }

    private void FixedUpdate()
    {
        if (client != null)
        {
            client.myInfo.playerPos = myPlayer.transform.position;
            hostPlayer.transform.position = client.hostInfo.playerPos;
        }
    }
}
