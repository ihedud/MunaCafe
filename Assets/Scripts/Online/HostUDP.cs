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
    
    private bool closed = true;
    private bool readyToPlay = false;
    private bool nextScene = false;
    private bool onLoad = false;
    [HideInInspector] public bool readyToListen = false;
    [HideInInspector] public bool pingDone = false;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread connectingThread;
    private Thread receivingThread;
    private Thread sendingThread;

    [HideInInspector] public Player myPlayer = new Player();
    [HideInInspector] public Player clientPlayer = new Player();

    private JsonSerialization json = new JsonSerialization();
    private LoadScene loader = new LoadScene();

    [SerializeField] private GameObject playButton;

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
        myPlayer.username = usernameInputField.GetComponent<TMP_InputField>().text;

        // Adding host to lobby
        playerManager.ConnectPlayer(myPlayer.username, playerCount);
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
            clientPlayer = json.JsonDeserialize(Encoding.ASCII.GetString(dataReceived1, 0, newSocket.ReceiveFrom(dataReceived1, ref remote)));

            Debug.Log(clientPlayer.username + " wants to connect...");

            // Adding client to lobby
            if (playerCount < 2)
            {
                playerManager.ConnectPlayer(clientPlayer.username, playerCount);
                playerCount++;
                Debug.Log(clientPlayer.username + " has joined the server!");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        // Send data
        byte[] dataSent1 = Encoding.ASCII.GetBytes(json.JsonSerialize(myPlayer));
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
                    clientPlayer = json.JsonDeserialize(Encoding.ASCII.GetString(dataReceived2, 0, newSocket.ReceiveFrom(dataReceived2, ref remote)));

                    if (clientPlayer.onPlay)
                        nextScene = true;

                    if (!clientPlayer.hasPing)
                        pingDone = false;
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
                    byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(myPlayer));
                    newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);
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
        myPlayer.onPlay = true;
    }
}