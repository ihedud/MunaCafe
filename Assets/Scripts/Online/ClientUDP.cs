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

    private Information myInfo = new Information();
    private Information hostInfo = new Information();

    [SerializeField] private JsonSerialization json;

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

        myInfo.username = username;

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
            dataSent = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
            recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

            // Receive host data
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            string hostData = Encoding.ASCII.GetString(dataReceived, 0, recv);
            hostInfo = json.JsonDeserialize(hostData);
            string hostUsername = hostInfo.username;

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
            newSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}