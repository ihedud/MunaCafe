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

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
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
                string[] dataSplit = data.Split(char.Parse("_"));
                string clientUsername = dataSplit[0];
                Debug.Log(clientUsername + " wants to connect...");

                // Adding client to lobby
                if(playerCount < 2)
                {
                    playerCount++;
                    playerManager.ConnectPlayer(clientUsername, playerCount);
                    Debug.Log(clientUsername + " has joined the server!");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            // Send data
            dataSent = Encoding.ASCII.GetBytes(username + "_" + playerManager.FindPlayer(username).emojiID);
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
}