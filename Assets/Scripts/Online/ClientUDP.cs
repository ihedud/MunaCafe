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

    private int recv;
    private bool closed = true;
    private bool readyToListen = false;

    private IPEndPoint host;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;
    private Thread listeningThread;
    private Thread peanutThread;

    private Information myInfo = new Information();
    private Information hostInfo = new Information();

    [SerializeField] private JsonSerialization json;
    [SerializeField] private LoadScene loader;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (myInfo.onPlay)
        {
            myInfo.onPlay = false;
            loader.LoadNextScene("ClientGame");
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
            byte[] dataSent = new byte[1024];
            dataSent = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
            recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

            // Receive host data
            byte[] dataReceived = new byte[1024];
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            string hostData = Encoding.ASCII.GetString(dataReceived, 0, recv);
            hostInfo = json.JsonDeserialize(hostData);
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
                // Receive new data
                byte[] dataReceived = new byte[1024];
                recv = newSocket.ReceiveFrom(dataReceived, ref remote);
                string data = Encoding.ASCII.GetString(dataReceived, 0, recv);
                hostInfo = json.JsonDeserialize(data);
                myInfo.onPlay = hostInfo.onPlay;
            }
        }
    }

    private void Peanut()
    {
        while (!closed)
        {
            if (readyToListen)
            {
                byte[] dataSent = new byte[1024];
                // Send data
                dataSent = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
                recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);
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