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
    //private List<Information> packetList = new List<Information>();
    //private bool resend = false;
    //private int resendID = 0;

    private bool closed = true;
    private bool readyToPlay = false;
    public bool readyToListen = false;
    private bool nextScene = false;
    public bool pingDone = false;
    public bool interactionDone = false;
    private bool onLoad = false;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread connectingThread;
    private Thread receivingThread;
    private Thread sendingThread;

    public Information myInfo = new Information();
    public Information clientInfo = new Information();

    [SerializeField] private GameObject playButton;
    [SerializeField] private LoadScene loader;
    [SerializeField] private JsonSerialization json;

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

                    //myInfo.clientPacketID = clientInfo.clientPacketID;

                    //for (int i = 0; i < packetList.Count; i++)
                    //{
                    //    if (packetList[i].hostPacketID == clientInfo.hostPacketID)
                    //        packetList.RemoveAt(i);
                        
                    //    if (clientInfo.hostPacketID > packetList[i].hostPacketID)
                    //    {
                    //        // Resend data
                    //        byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(packetList[i]));
                    //        newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);
                    //        //resendID = packetList[i].hostPacketID;
                    //        //resend = true;
                    //    }
                    //}

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
                    // Send data
                    //myInfo.hostPacketID++;
                    byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
                    newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);
                    // if list.count mayor que 10, send world state y clear list
                    //if (packetList.Count > 10)
                    //    Debug.Log("worldstate");
                    
                    //if (packetList.Count < 200) packetList.Add(myInfo);
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