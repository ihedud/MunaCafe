using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] private GameObject myPlayer;
    [SerializeField] private GameObject hostPlayer;
    [HideInInspector] [SerializeField] private bool lobby = false;
    [SerializeField] private List<Material> materials;
    [SerializeField] private TrayClient tray1;
    [SerializeField] private TrayClient tray2;
    [SerializeField] private DonutStation donutStation;
    private ClientUDP client;

    public ClientUDP clientInfo => client;
    [HideInInspector] public int colorID;

    private void Awake()
    {
        client = FindObjectOfType<ClientUDP>();
        if (!lobby)
        {
            myPlayer.GetComponentInChildren<MeshRenderer>().material = materials[client.myInfo.colorID];
            hostPlayer.GetComponentInChildren<MeshRenderer>().material = materials[client.hostInfo.colorID];
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
                    // Color
                    client.myInfo.colorID = colorID;
                    hostPlayer.GetComponent<MeshRenderer>().material = materials[client.hostInfo.colorID];
                }
            }
            else
            {
                // Position
                hostPlayer.transform.position = client.hostInfo.playerPos;
                client.myInfo.playerPos = myPlayer.transform.position;

                // Ping
                if (client.hostInfo.hasPing && !client.pingDone)
                {
                    hostPlayer.GetComponent<PlayerCommunicating>().ShowPingFromMessage();
                    client.pingDone = true;
                }

                client.myInfo.hasPing = myPlayer.GetComponent<PlayerCommunicating>().isShowing;

                // Interaction
                hostPlayer.GetComponent<PlayerState>().interactionDone = client.interactionDone;
                hostPlayer.GetComponent<PlayerState>().hasInteracted = client.hostInfo.hasInteracted;

                if(client.hostInfo.hasInteracted && !client.interactionDone)
                    client.interactionDone = true;

                client.myInfo.hasInteracted = myPlayer.GetComponent<PlayerState>().hasInteracted;

                // Tray
                tray1.currentOrder = (TrayClient.Order)client.hostInfo.order1;
                tray2.currentOrder = (TrayClient.Order)client.hostInfo.order2;

                // Donut
                client.myInfo.donutMesh = donutStation.myCurrentMesh;
                donutStation.otherCurrentMesh = client.hostInfo.donutMesh;
            }
        }
    }
}
