using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    [SerializeField] private GameObject myPlayer;
    [SerializeField] private GameObject clientPlayer;
    private HostUDP host;

    private void Awake()
    {
        host = FindObjectOfType<HostUDP>();
    }

    private void FixedUpdate()
    {
        if (host != null)
        {
            clientPlayer.transform.position = host.clientInfo.playerPos;
            host.myInfo.playerPos = myPlayer.transform.position;
        }
    }
}
