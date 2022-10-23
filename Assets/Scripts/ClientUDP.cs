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
    [SerializeField] private string ipAddress = "127.0.0.1";
    [SerializeField] private int port = 9050;

    [Header("Session Data")]
    [SerializeField] private GameObject serverIPInputField;
    [SerializeField] private GameObject usernameInputField;

    private string serverIP;
    private string username;

    private int recv;
    private string message;
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
            Debug.LogWarning("Starting Thread");
            Debug.Log("Sending Message");

            host = new IPEndPoint(IPAddress.Parse(serverIP), port);
            remote = (EndPoint)host;

            // Send Data
            message = "Hi, I want to connect!";
            dataSent = Encoding.Default.GetBytes(message);
            recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

            // Receive Data
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            Debug.Log(Encoding.ASCII.GetString(dataReceived, 0, recv));
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

