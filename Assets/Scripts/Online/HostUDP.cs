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

    private string username;
    private int playerCount = 0;

    private int recv;
    
    private bool closed = true;
    private bool readyToPlay = false;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;
    private Thread listeningThread;
    private Thread peanutThread;

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
        // Get data from session
        username = usernameInputField.GetComponent<TMP_InputField>().text;

        // Adding host to lobby
        playerManager.ConnectPlayer(username, playerCount);
        playerManager.hostUpdated = true;

        // Initialize socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        closed = false;

        myInfo.username = username;

        // Initialize thread
        myThread = new Thread(HostConnection);
        myThread.Start();

        // Listening thread
        listeningThread = new Thread(ListeningClient);
        listeningThread.Start();

        // Peanut thread
        peanutThread = new Thread(Peanut);
        peanutThread.Start();
    }

    private void HostConnection()
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
            recv = newSocket.ReceiveFrom(dataReceived1, ref remote);
            string data = Encoding.ASCII.GetString(dataReceived1, 0, recv);
            clientInfo = json.JsonDeserialize(data);
            string clientUsername = clientInfo.username;
            Debug.Log(clientUsername + " wants to connect...");
            
            // Adding client to lobby
            if(playerCount < 2)
            {
                playerCount++;
                playerManager.ConnectPlayer(clientUsername, playerCount);
                Debug.Log(clientUsername + " has joined the server!");
                
                readyToPlay = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        // Send data
        byte[] dataSent1 = Encoding.ASCII.GetBytes(json.JsonSerialize(myInfo));
        newSocket.SendTo(dataSent1, dataSent1.Length, SocketFlags.None, remote);
    }

    private void ListeningClient()
    {
        while (!closed)
        {
            try
            {
                // Receive data
                byte[] dataReceived2 = new byte[1024];
                recv = newSocket.ReceiveFrom(dataReceived2, ref remote);
                string data = Encoding.ASCII.GetString(dataReceived2, 0, recv);
                clientInfo = json.JsonDeserialize(data);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    private void Peanut()
    {
        while (!closed)
        {
            try
            {
                // Send data
                byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
                recv = newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
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

    public void OnPlayGame()
    {
        myInfo.onPlay = true;

        // Send data
        byte[] dataSent3 = Encoding.ASCII.GetBytes(json.JsonSerialize(myInfo));
        newSocket.SendTo(dataSent3, dataSent3.Length, SocketFlags.None, remote);

        myInfo.onPlay = false;

        loader.LoadNextScene("HostGame");
    }
}