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
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i] == myPlayer.GetComponent<MeshRenderer>().material)
                        client.myInfo.colorID = i;
                }
            }
            else
            {
                if (myPlayer != null)
                {
                    client.myInfo.playerPos = myPlayer.transform.position;
                    hostPlayer.transform.position = client.hostInfo.playerPos;
                }
            }
        }
    }
}
