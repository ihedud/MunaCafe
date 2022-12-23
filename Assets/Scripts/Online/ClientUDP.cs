﻿using System.Collections;
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
    private int playerCount = 0;
    private List<Information> packetList = new List<Information>();

    private bool closed = true;
    public bool readyToListen = false;
    private bool nextScene = false;
    public bool pingDone = false;
    public bool interactionDone = false;
    private bool onLoad = false;

    private IPEndPoint host;
    private EndPoint remote;
    private Socket newSocket;
    private Thread connectingThread;
    private Thread receivingThread;
    private Thread sendingThread;

    public Information myInfo = new Information();
    public Information hostInfo = new Information();

    [SerializeField] private JsonSerialization json;
    [SerializeField] private LoadScene loader;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (nextScene && !onLoad)
        {
            onLoad = true;
            nextScene = false;
            myInfo.onPlay = true;
            loader.LoadNextScene("ClientGame");
        }
    }

    public void Initialize()
    {
        // Get data from session
        serverIP = serverIPInputField.GetComponent<TMP_InputField>().text;
        myInfo.username = usernameInputField.GetComponent<TMP_InputField>().text;

        // Initialize socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        closed = false;

        // Connecting thread
        connectingThread = new Thread(Connecting);
        connectingThread.Start();

        // Receiving thread
        receivingThread = new Thread(Receiving);
        receivingThread.Start();

        // Sending thread
        sendingThread = new Thread(Sending);
        sendingThread.Start();
    }

    private void Connecting()
    {
        try
        {
            host = new IPEndPoint(IPAddress.Parse(serverIP), port);
            remote = (EndPoint)host;

            // Send data
            byte[] dataSent1 = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
            newSocket.SendTo(dataSent1, dataSent1.Length, SocketFlags.None, remote);

            // Receive data
            byte[] dataReceived1 = new byte[1024];
            hostInfo = json.JsonDeserialize(Encoding.ASCII.GetString(dataReceived1, 0, newSocket.ReceiveFrom(dataReceived1, ref remote)));

            // Adding host and client to lobby
            playerManager.ConnectPlayer(hostInfo.username, playerCount);
            playerCount++;
            playerManager.ConnectPlayer(myInfo.username, playerCount);

            readyToListen = true;
        }
        catch (Exception e)
        {
            Debug.Log("Server is not open yet. Error: " + e.Message);
        }
    }

    private void Receiving()
    {
        while (!closed)
        {
            if (readyToListen)
            {
                try
                {
                    // Receive data
                    byte[] dataReceived2 = new byte[1024];
                    hostInfo = json.JsonDeserialize(Encoding.ASCII.GetString(dataReceived2, 0, newSocket.ReceiveFrom(dataReceived2, ref remote)));

                    myInfo.hostPacketID = hostInfo.hostPacketID;

                    for (int i = 0; i < packetList.Count; i++)
                    {
                        if (packetList[i].clientPacketID == hostInfo.clientPacketID)
                            packetList.RemoveAt(i);

                        if (hostInfo.clientPacketID > packetList[i].clientPacketID)
                        {
                            // Resend data
                            byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(packetList[i]));
                            newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);
                            //resendID = packetList[i].hostPacketID;
                            //resend = true;
                        }
                    }

                    if (hostInfo.onPlay)
                        nextScene = true;

                    if (!hostInfo.hasPing)
                        pingDone = false;

                    if (!hostInfo.hasInteracted)
                        interactionDone = false;
                }
                catch (Exception e) 
                { 
                    Debug.Log(e.Message); 
                }
            }
        }
    }

    private void Sending()
    {
        while (!closed)
        {
            if (readyToListen)
            {
                try
                {
                    // Send data
                    myInfo.clientPacketID++;
                    byte[] dataSent2 = Encoding.Default.GetBytes(json.JsonSerialize(myInfo));
                    newSocket.SendTo(dataSent2, dataSent2.Length, SocketFlags.None, remote);
                    if (packetList.Count > 10)
                        Debug.Log("pls send worldstate");
                    if (packetList.Count < 200) packetList.Add(myInfo);
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

        try
        {
            connectingThread.Abort();
            receivingThread.Abort();
            sendingThread.Abort();
            newSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}