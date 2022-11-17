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
    private byte[] dataSent = new byte[1024];
    private byte[] dataReceived = new byte[1024];
    private bool closed = true;
    private bool readyToPlay = false;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;

    private Information myInfo = new Information();
    private Information clientInfo = new Information();

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
            playButton.SetActive(true);
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
    }

    private void HostConnection()
    {
        Debug.Log("Starting Thread");

        client = new IPEndPoint(IPAddress.Any, port);
        remote = (EndPoint)client;

        newSocket.Bind(client);

        while (!closed)
        {
            try
            {
                Debug.Log("Waiting for clients...");
                
                // Receive data
                recv = newSocket.ReceiveFrom(dataReceived, ref remote);
                string data = Encoding.ASCII.GetString(dataReceived, 0, recv);
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
            dataSent = Encoding.ASCII.GetBytes(json.JsonSerialize(myInfo));
            newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);
        }
    }

    private void OnDisable()
    {
        closed = true;

        try
        {
            myThread.Abort();
            newSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnPlayGame()
    {
        dataSent = Encoding.ASCII.GetBytes(username);
        newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

        loader.LoadNextScene("HostGame");
    }
}