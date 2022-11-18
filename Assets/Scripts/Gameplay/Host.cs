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

    private void Awake()
    {
        host = FindObjectOfType<HostUDP>();

    }

    private void FixedUpdate()
    {
        if (host != null)
        {
            if (lobby)
            {
                clientPlayer.GetComponent<MeshRenderer>().material = materials[host.clientInfo.colorID];
            }
            else
            {
                clientPlayer.transform.position = host.clientInfo.playerPos;
                host.myInfo.playerPos = myPlayer.transform.position;
            }
        }
    }
}
