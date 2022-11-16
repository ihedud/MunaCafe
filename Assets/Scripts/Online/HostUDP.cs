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
    private string dataReceivedTemp;
    private bool closed = true;
    private bool startReceivingEmoji = false;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;
    private Thread emojiThread;

    private List<EndPoint> remotes = new List<EndPoint>();

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

        // Emoji thread
        emojiThread = new Thread(ReceivingEmoji);
        emojiThread.Start();
    }

    private void HostConnection()
    {
        Debug.Log("Starting Thread");

        client = new IPEndPoint(IPAddress.Any, port);
        remote = (EndPoint)client;

        newSocket.Bind(client);

        while (!closed)
        {
            if (!remotes.Contains(remote))
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

                    remotes.Add(remote);

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
            else
            {
                startReceivingEmoji = true;
            }

            if (playerManager.playerUpdated && remotes.Count > 0)
            {
                if (playerManager.hostUpdated)
                {
                    // Send data to all clients from host
                    for (int i = 0; i < remotes.Count; i++)
                    {
                        byte[] dataSentHost = new byte[1024];
                        dataSentHost = Encoding.Default.GetBytes(username + "_" + playerManager.FindPlayer(username).emojiID);
                        newSocket.SendTo(dataSentHost, dataSentHost.Length, SocketFlags.None, remotes[i]);

                        playerManager.hostUpdated = false;
                    }
                }
                
                playerManager.playerUpdated = false;
            }
        }
    }

    private void ReceivingEmoji()
    {
        while(!closed)
        {
            if(startReceivingEmoji)
            {
                try
                {
                    // Receive data
                    recv = newSocket.ReceiveFrom(dataReceived, ref remote);
                    dataReceivedTemp = Encoding.ASCII.GetString(dataReceived, 0, recv);
                    string[] dataSplit = dataReceivedTemp.Split(char.Parse("_"));
                    string clientUsername = dataSplit[0];
                    string clientEmojiID = dataSplit[1];

                    if (clientUsername != username && int.Parse(clientEmojiID) > -1)
                        playerManager.ShowEmoji(clientUsername, int.Parse(clientEmojiID));

                    Debug.Log(clientUsername + " is sending an emoji...");

                    playerManager.playerUpdated = true;
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
        startReceivingEmoji = false;

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