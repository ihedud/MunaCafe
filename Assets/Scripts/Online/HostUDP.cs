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

            if (!remotes.Contains(remote))
            {
                // Receive Data
                recv = newSocket.ReceiveFrom(dataReceived, ref remote);
                string data = Encoding.ASCII.GetString(dataReceived, 0, recv);
                string[] dataSplit = data.Split(char.Parse("_"));
                string clientUsername = dataSplit[0];
                string clientEmojiID = dataSplit[1]; // -1
                Debug.Log(clientUsername + " wants to connect...");

                playerCount++;
                remotes.Add(remote);
                playerManager.ConnectPlayer(clientUsername, playerCount);
                Debug.Log(clientUsername + " has joined the server!");

                // Send Data
                dataSent = Encoding.ASCII.GetBytes(/*serverName + "_" + */username + "_" + playerManager.FindPlayer(username).emojiID);
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
                    // Send Data To All Clients From Host
                    for (int i = 0; i < remotes.Count; i++)
                    {
                        byte[] dataSentHost = new byte[1024];
                        dataSentHost = Encoding.Default.GetBytes(username + "_" + playerManager.FindPlayer(username).emojiID);
                        newSocket.SendTo(dataSentHost, dataSentHost.Length, SocketFlags.None, remotes[i]);

                        playerManager.hostUpdated = false;
                    }
                }
                else
                {
                    // Send Data To All Clients From Other Clients
                    if (dataReceivedTemp != null)
                    {
                        for (int i = 0; i < remotes.Count; i++)
                        {
                            byte[] dataSent2 = new byte[1024];
                            dataSent2 = Encoding.Default.GetBytes(dataReceivedTemp);
                            newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remotes[i]);
                        }
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
                // Receive Data
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
        }
    }

    public void Initializing()
    {
        // Get Data From Session
        serverName = serverNameInputField.GetComponent<TMP_InputField>().text;
        username = usernameInputField.GetComponent<TMP_InputField>().text;
        playerManager.ConnectPlayer(username, playerCount);
        playerManager.hostUpdated = true;

        // Initialize Socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Initialize Thread
        closed = false;
        myThread = new Thread(HostConnection);
        myThread.Start();

        // Emoji Listen
        emojiThread = new Thread(ReceivingEmoji);
        emojiThread.Start();
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
