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

    private IPEndPoint host;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;

    private void ClientConnection()
    {
        try
        {
            Debug.Log("Starting Thread");
            Debug.Log("Sending Message");

            host = new IPEndPoint(IPAddress.Parse(serverIP), port);
            remote = (EndPoint)host;

            // Send Data
            dataSent = Encoding.Default.GetBytes(username);
            recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

            // Receive Data
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            Debug.Log("server name: " + Encoding.ASCII.GetString(dataReceived, 0, recv));
            Debug.Log(remote.ToString());
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            string clientUsername = Encoding.ASCII.GetString(dataReceived, 0, recv);

            playerCount++;
            playerManager.ConnectPlayer(clientUsername, playerCount);
            Debug.Log(clientUsername + " has joined the server!");
        }
        catch
        {
            Debug.Log("Server is not open yet.");

            myThread.Abort();
            newSocket.Close();
        }
    }

    public void Initialize()
    {
        // Get Data From Session
        serverIP = serverIPInputField.GetComponent<TMP_InputField>().text;
        username = usernameInputField.GetComponent<TMP_InputField>().text;
        playerManager.ConnectPlayer(username, playerCount);

        // Initialize Socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Initialize Thread
        myThread = new Thread(ClientConnection);
        myThread.Start();
    }

    private void OnDisable()
    {
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