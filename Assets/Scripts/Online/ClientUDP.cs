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
    private byte[] dataSent = new byte[1024];
    private byte[] dataReceived = new byte[1024];
    private bool closed = true;

    private IPEndPoint host;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;
    private Thread emojiThread;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Initialize()
    {
        // Get data from session
        serverIP = serverIPInputField.GetComponent<TMP_InputField>().text;
        username = usernameInputField.GetComponent<TMP_InputField>().text;

        // Initialize socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        closed = false;

        // Initialize thread
        myThread = new Thread(ClientConnection);
        myThread.Start();
    }

    private void ClientConnection()
    {
        try
        {
            host = new IPEndPoint(IPAddress.Parse(serverIP), port);
            remote = (EndPoint)host;

            // Send data
            dataSent = Encoding.Default.GetBytes(username + "_" + "7");
            recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

            // Receive host data
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            string hostData = Encoding.ASCII.GetString(dataReceived, 0, recv);
            string[] hostDataSplit = hostData.Split(char.Parse("_"));
            string hostUsername = hostDataSplit[0];

            // Adding host and client to lobby
            playerManager.ConnectPlayer(hostUsername, playerCount);
            playerCount++;
            playerManager.ConnectPlayer(username, playerCount);
        }
        catch (Exception e)
        {
            Debug.Log("Server is not open yet. Error: " + e.Message);
        }
    }

    private void OnDisable()
    {
        closed = true;

        try
        {
            myThread.Abort();
            emojiThread.Abort();
            newSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}