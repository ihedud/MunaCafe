using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    [SerializeField] private GameObject myPlayer;
    [SerializeField] private GameObject clientPlayer;
    [HideInInspector] [SerializeField] private bool lobby = false;
    [SerializeField] private List<Material> materials;
    [SerializeField] private Tray tray1;
    [SerializeField] private Tray tray2;
    [SerializeField] private DonutStation donutStation;
    private HostUDP host;

    public HostUDP hostInfo => host;
    [HideInInspector] public int colorID;

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
                    // Color
                    host.myInfo.colorID = colorID;
                    clientPlayer.GetComponent<MeshRenderer>().material = materials[host.clientInfo.colorID];
                }
            }
            else
            {
                // Position
                clientPlayer.transform.position = host.clientInfo.playerPos;
                host.myInfo.playerPos = myPlayer.transform.position;

                // Ping
                if (host.clientInfo.hasPing && !host.pingDone)
                {
                    clientPlayer.GetComponent<PlayerCommunicating>().ShowPingFromMessage();
                    host.pingDone = true;
                }

                host.myInfo.hasPing = myPlayer.GetComponent<PlayerCommunicating>().isShowing;

                // Interaction
                clientPlayer.GetComponent<PlayerState>().interactionDone = host.interactionDone;
                clientPlayer.GetComponent<PlayerState>().hasInteracted = host.clientInfo.hasInteracted;

                if (host.clientInfo.hasInteracted && !host.interactionDone)
                    host.interactionDone = true;

                host.myInfo.hasInteracted = myPlayer.GetComponent<PlayerState>().hasInteracted;

                // Tray
                host.myInfo.order1 = (int)tray1.currentOrder;
                host.myInfo.order2 = (int)tray2.currentOrder;
            }
        }
    }
}
