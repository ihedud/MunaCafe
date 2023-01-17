using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class HostUDP : MonoBehaviour
{
    [SerializeField] private int port = 9050;

    [Header("Session Data")]
    [SerializeField] private GameObject serverNameInputField;
    [SerializeField] private GameObject usernameInputField;
    [SerializeField] private ManagePlayers playerManager;

    private int playerCount = 0;
    private List<Information> packetList = new List<Information>();

    [HideInInspector] public bool readyToListen = false;
    [HideInInspector] public bool pingDone = false;
    [HideInInspector] public bool interactionDone = false;
    private bool onLoad = false;
    private bool nextScene = false;
    private bool readyToPlay = false;
    private bool closed = true;
    private bool hasAlreadyInteracted = false;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread connectingThread;
    private Thread receivingThread;
    private Thread sendingThread;

    [HideInInspector] public Information myInfo = new Information();
    [HideInInspector] public Information clientInfo = new Information();
    private Information lostPacket = null;

    [SerializeField] private GameObject playButton;
    [SerializeField] private LoadScene loader;
    [SerializeField] private JsonSerialization json;

    private int timer = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (nextScene && !onLoad)
        {
            onLoad = true;
            nextScene = false;
            loader.LoadNextScene("HostGame");
        }

        if (!readyToPlay)
            return;

        if (playButton != null)
        {
            playButton.SetActive(true);
            readyToPlay = false;
        }
    }

    public void Initializing()
    {
        myInfo.username = usernameInputField.GetComponent<TMP_InputField>().text;

        // Adding host to lobby
        playerManager.ConnectPlayer(myInfo.username, playerCount);
        playerCount++;
        playerManager.hostUpdated = true;

        // Initialize socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        closed = false;

        // Connecting thread
        connectingThread = new Thread(Connecting);
        connectingThread.Start();

        // Receiving thread
        receivingThread = new Thread(Receiving);
        receivingThread.Start();

        // Sending thread
        sendingThread = new Thread(Sending);
        sendingThread.Start();
    }

    private void Connecting()
    {
        Debug.Log("Starting Thread");

        client = new IPEndPoint(IPAddress.Any, port);
        remote = (EndPoint)client;

        newSocket.Bind(client);

        try
        {
            Debug.Log("Waiting for clients...");

            // Receive data
            byte[] dataReceived1 = new byte[1024];
            clientInfo = json.JsonDeserialize(Encoding.ASCII.GetString(dataReceived1, 0, newSocket.ReceiveFrom(dataReceived1, ref remote)));

            Debug.Log(clientInfo.username + " wants to connect...");

            // Adding client to lobby
            if (playerCount < 2)
            {
                playerManager.ConnectPlayer(clientInfo.username, playerCount);
                playerCount++;
                Debug.Log(clientInfo.username + " has joined the server!");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        // Send data
        byte[] dataSent1 = Encoding.ASCII.GetBytes(json.JsonSerialize(myInfo));
        newSocket.SendTo(dataSent1, dataSent1.Length, SocketFlags.None, remote);

        if (playerCount == 2)
        { 
            readyToPlay = true;
            readyToListen = true;
        }
    }

    private void Receiving()
    {
        while (!closed)
        {
            if (readyToListen)
            {
                try
                {
                    // Receive data
                    byte[] dataReceived2 = new byte[1024];
                    clientInfo = json.JsonDeserialize(Encoding.ASCII.GetString(dataReceived2, 0, newSocket.ReceiveFrom(dataReceived2, ref remote)));

                    //Debug.Log("Receiving " + clientInfo.hostPacketID);
                    myInfo.clientPacketID = clientInfo.clientPacketID;

                    if (clientInfo.onPlay)
                        nextScene = true;

                    if (!clientInfo.hasPing)
                        pingDone = false;

                    if (!clientInfo.hasInteracted)
                        interactionDone = false;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
    }

    private void Sending()
    {
        while (!closed)
        {
            if (readyToListen)
            {
                try
                {
                    // Solo las veces que hemos interactuado
                    for (int i = 0; i < packetList.Count; i++)
                    {
                        if (packetList[i].hostPacketID == clientInfo.hostPacketID)
                        {
                            packetList.RemoveAt(i);
                            hasAlreadyInteracted = false;
                        }

                        if (clientInfo.hostPacketID > packetList[i].hostPacketID)
                        {
                            lostPacket = packetList[i];
                        }
                    }

                    timer++;
                    
                        

                        // Send data
                        if (lostPacket != null)
                        {
                            Debug.Log("Resending lost packet: " + lostPacket.hostPacketID);
                            byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(lostPacket));
                            newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);
                            lostPacket = null;
                        }
                        else if (lostPacket == null && (timer >= 1000 || myInfo.hasInteracted))
                        {
                        timer = 0;
                        myInfo.hostPacketID++;
                            byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
                            newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);

                            if (myInfo.hasInteracted && !hasAlreadyInteracted)
                            {
                                hasAlreadyInteracted = true;
                                packetList.Add(myInfo);
                                Debug.Log("Adding packet to list: " + myInfo.hostPacketID);
                            }
                        }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
    }

    private void OnDisable()
    {
        closed = true;

        try
        {
            connectingThread.Abort();
            receivingThread.Abort();
            sendingThread.Abort();
            newSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnPlayGame()
    {
        myInfo.onPlay = true;
    }
}