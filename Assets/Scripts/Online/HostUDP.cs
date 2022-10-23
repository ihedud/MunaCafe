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

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;
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
                // Receive Data
                recv = newSocket.ReceiveFrom(dataReceived, ref remote);
                string dataReceivedTemp = Encoding.ASCII.GetString(dataReceived, 0, recv);
                string[] dataSplit = dataReceivedTemp.Split(char.Parse("_"));
                string clientUsername = dataSplit[0];
                string clientEmojiID = dataSplit[1];

                Debug.Log(clientUsername + " is sending an emoji...");

                playerManager.playerUpdated = true;
            }

            if (playerManager.playerUpdated && remotes.Count > 0)
            {
                // Send Data To All Clients
                for (int i = 0; i < remotes.Count; i++)
                {
                    byte[] dataSent2 = new byte[1024];
                    dataSent2 = Encoding.Default.GetBytes(dataReceivedTemp);
                    newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remotes[i]);
                }
                playerManager.playerUpdated = false;
            }
        }
    }

    public void Initializing()
    {
        // Get Data From Session
        serverName = serverNameInputField.GetComponent<TMP_InputField>().text;
        username = usernameInputField.GetComponent<TMP_InputField>().text;
        playerManager.ConnectPlayer(username, playerCount);

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
