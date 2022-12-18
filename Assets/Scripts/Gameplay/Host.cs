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
            myPlayer.GetComponentInChildren<MeshRenderer>().material = materials[host.myInfo.colorID];
            clientPlayer.GetComponentInChildren<MeshRenderer>().material = materials[host.clientInfo.colorID];
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
                    host.myInfo.colorID = colorID;
                    clientPlayer.GetComponent<MeshRenderer>().material = materials[host.clientInfo.colorID];
                }
            }
            else
            {
                clientPlayer.transform.position = host.clientInfo.playerPos;
                host.myInfo.playerPos = myPlayer.transform.position;

                if (host.clientInfo.hasPing && !host.pingDone)
                {
                    clientPlayer.GetComponent<PlayerCommunicating>().ShowPingFromMessage();
                    host.pingDone = true;
                }

                if (host.clientInfo.hasInteracted)
                {
                    clientPlayer.GetComponent<PlayerState>().hasInteracted = host.clientInfo.hasInteracted;
                }

                if (myPlayer.GetComponent<PlayerCommunicating>().isShowing)
                    host.myInfo.hasPing = true;
                else
                    host.myInfo.hasPing = false;
            }
        }
    }
}
