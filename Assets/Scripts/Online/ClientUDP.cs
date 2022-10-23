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

    public int emojiID = -1;

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
    private void Awake()
    {
        emojiID = -1;
    }
    private void ClientConnection()
    {
        try
        {
            Debug.Log("Starting Thread");
            Debug.Log("Sending Message");

            host = new IPEndPoint(IPAddress.Parse(serverIP), port);
            remote = (EndPoint)host;

            // Send Data
            dataSent = Encoding.Default.GetBytes(username + "_" + "-1");
            recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

            // Receive Host Data
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            Debug.Log(Encoding.ASCII.GetString(dataReceived, 0, recv));
            string hostData = Encoding.ASCII.GetString(dataReceived, 0, recv);
            string[] hostDataSplit = hostData.Split(char.Parse("_"));
            string hostUsername = hostDataSplit[0];
            string hostEmojiID = hostDataSplit[1];

            playerManager.ConnectPlayer(hostUsername, playerCount);
            playerCount++;
            playerManager.ConnectPlayer(username, playerCount);

            while (!closed)
            {
                if (playerManager.emojiUpdated)
                {
                    // Send Data
                    dataSent = Encoding.Default.GetBytes(username + "_" + playerManager.FindPlayer(username).emojiID);
                    recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

                    playerManager.emojiUpdated = false;
                }

                // Receive New Data
                recv = newSocket.ReceiveFrom(dataReceived, ref remote);
                string data = Encoding.ASCII.GetString(dataReceived, 0, recv);
                string[] dataSplit = data.Split(char.Parse("_"));
                string clientUsername = dataSplit[0];
                string clientEmojiID = dataSplit[1];

                Debug.Log(clientUsername + " has joined the server!");

                playerCount++;
                playerManager.ConnectPlayer(clientUsername, playerCount);
            }
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

        // Initialize Socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Initialize Thread
        closed = false;
        myThread = new Thread(ClientConnection);
        myThread.Start();
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