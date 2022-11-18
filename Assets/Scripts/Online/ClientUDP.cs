using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    [SerializeField] private int port = 9050;

    [Header("Session Data")]
    [SerializeField] private GameObject serverIPInputField;
    [SerializeField] private GameObject usernameInputField;
    [SerializeField] private ManagePlayers playerManager;

    private string serverIP;
    private string username;
    private int playerCount = 0;

    private bool closed = true;
    public bool readyToListen = false;
    private bool nextScene = false;

    private IPEndPoint host;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;
    private Thread listeningThread;
    private Thread peanutThread;

    public Information myInfo = new Information();
    public Information hostInfo = new Information();

    [SerializeField] private JsonSerialization json;
    [SerializeField] private LoadScene loader;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (nextScene)
        {
            nextScene = false;
            loader.LoadNextScene("ClientGame");
            myInfo.onPlay = true;
        }
    }

    public void Initialize()
    {
        // Get data from session
        serverIP = serverIPInputField.GetComponent<TMP_InputField>().text;
        username = usernameInputField.GetComponent<TMP_InputField>().text;

        // Initialize socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        closed = false;

        myInfo.username = username;

        // Initialize thread
        myThread = new Thread(ClientConnection);
        myThread.Start();

        // Listening thread
        listeningThread = new Thread(ListeningHost);
        listeningThread.Start();

        // Peanut thread
        peanutThread = new Thread(Peanut);
        peanutThread.Start();
    }

    private void ClientConnection()
    {
        try
        {
            host = new IPEndPoint(IPAddress.Parse(serverIP), port);
            remote = (EndPoint)host;

            // Send data
            byte[] dataSent1 = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
            newSocket.SendTo(dataSent1, dataSent1.Length, SocketFlags.None, remote);

            // Receive data
            byte[] dataReceived1 = new byte[1024];
            hostInfo = json.JsonDeserialize(Encoding.ASCII.GetString(dataReceived1, 0, newSocket.ReceiveFrom(dataReceived1, ref remote)));

            string hostUsername = hostInfo.username;

            // Adding host and client to lobby
            playerManager.ConnectPlayer(hostUsername, playerCount);
            playerCount++;
            playerManager.ConnectPlayer(username, playerCount);

            readyToListen = true;
        }
        catch (Exception e)
        {
            Debug.Log("Server is not open yet. Error: " + e.Message);
        }
    }

    private void ListeningHost()
    {
        while (!closed)
        {
            if (readyToListen)
            {
                try
                {
                    // Receive data
                    byte[] dataReceived2 = new byte[1024];
                    hostInfo = json.JsonDeserialize(Encoding.ASCII.GetString(dataReceived2, 0, newSocket.ReceiveFrom(dataReceived2, ref remote)));

                    if (hostInfo.onPlay)
                        nextScene = true;
                }
                catch (Exception e) 
                { 
                    Debug.Log(e.Message); 
                }
            }
        }
    }

    private void Peanut()
    {
        while (!closed)
        {
            if (readyToListen)
            {
                try
                {
                    // Send data
                    byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
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
            myThread.Abort();
            listeningThread.Abort();
            peanutThread.Abort();
            newSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}