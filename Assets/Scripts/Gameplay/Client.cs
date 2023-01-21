using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private GameObject myPlayer;
    [SerializeField] private GameObject hostPlayer;
    [HideInInspector] [SerializeField] private bool lobby = false;
    [SerializeField] private List<Material> materials;

    [Header("Machines")]
    [SerializeField] private Tray tray1;
    [SerializeField] private Tray tray2;
    [SerializeField] private CoffeeMachine coffeeMachine1;
    [SerializeField] private CoffeeMachine coffeeMachine2;
    [SerializeField] private CoffeeMachine coffeeMachine3;
    [SerializeField] private TeaPot teaPot;
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
                client.myInfo.order1 = (int)tray1.currentOrder;
                client.myInfo.order2 = (int)tray2.currentOrder;

                // Machines State

                client.myInfo.coffee1State = coffeeMachine1.CurrentState;
                client.myInfo.coffee2State = coffeeMachine2.CurrentState;
                client.myInfo.coffee3State = coffeeMachine3.CurrentState;

                coffeeMachine1.newState = client.hostInfo.coffee1State;
                coffeeMachine2.newState = client.hostInfo.coffee2State;
                coffeeMachine3.newState = client.hostInfo.coffee3State;

                client.myInfo.donutState = donutStation.CurrentState;

                donutStation.newState = client.hostInfo.donutState;

                client.myInfo.teaPotState = teaPot.CurrentState;

                teaPot.newState = client.hostInfo.teaPotState;
            }
        }
    }
}
