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

    public int emojiID = -1;

    private string serverName;
    private string username;
    private int playerCount = 0;

    private int recv;
    private byte[] dataSent = new byte[1024];
    private byte[] dataSent2 = new byte[1024];
    private byte[] dataReceived = new byte[1024];
    private bool closed = true;
    private bool playerUpdated = false;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;
    private Dictionary<EndPoint, int> remotes = new Dictionary<EndPoint, int>();
    //private static PlayerToByte playerToByte;

    private void Awake()
    {
        emojiID = -1;
    }

    private void HostConnection()
    {
        Debug.Log("Starting Thread");

        client = new IPEndPoint(IPAddress.Any, port);
        remote = (EndPoint)client;
        
        newSocket.Bind(client);

        while (!closed)
        {
            Debug.Log("Waiting for clients...");

            
            if (!remotes.ContainsKey(remote))
            {
                // Receive Data
                recv = newSocket.ReceiveFrom(dataReceived, ref remote);
                Debug.Log(remote.ToString());
                string clientUsername = Encoding.ASCII.GetString(dataReceived, 0, recv);
                Debug.Log(clientUsername + " wants to connect...");

                playerCount++;
                remotes.Add(remote, playerCount);
                playerManager.ConnectPlayer(clientUsername, playerCount);
                Debug.Log(clientUsername + " has joined the server!");

                // Send Data
                dataSent = Encoding.ASCII.GetBytes(serverName);
                newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

                dataSent2 = Encoding.ASCII.GetBytes(username);
                newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);
            }
            else
            {
                // Receive Data
                recv = newSocket.ReceiveFrom(dataReceived, ref remote);
                Debug.Log(remote.ToString());
                string clientUsername = Encoding.ASCII.GetString(dataReceived, 0, recv);
                Debug.Log(clientUsername + " wants to connect...");
            }

            if (playerUpdated)
            {
                // Send Data
                for (int i = 0; i < remotes.Count; i++)
                {
                    
                    //newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remotes[i]);
                }
                playerUpdated = false;
            }
        }
    }

    private void Update()
    {
        if (emojiID >= 0)
            playerManager.ShowEmoji(username, emojiID);
    }

    public void Initializing()
    {
        // Get Data From Session
        serverName = serverNameInputField.GetComponent<TMP_InputField>().text;
        username = usernameInputField.GetComponent<TMP_InputField>().text;
        playerManager.ConnectPlayer(username, playerCount);
        playerUpdated = true;

    // Initialize Socket
    newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Initialize Thread
        closed = false;
        myThread = new Thread(HostConnection);
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
