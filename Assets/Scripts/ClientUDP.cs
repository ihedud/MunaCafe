using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;

public class ClientUDP : MonoBehaviour
{
    [SerializeField] private string ipAddress = "127.0.0.1";
    [SerializeField] private int port = 9050;

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
        Debug.LogWarning("Starting Thread");
        Debug.Log("Sending Message");

        host = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        remote = (EndPoint)host;

        // Send Data
        message = "Hi, I want to connect!";
        dataSent = Encoding.Default.GetBytes(message);
        recv = newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);

        // Receive Data
        recv = newSocket.ReceiveFrom(dataReceived, ref remote);
        Debug.Log(Encoding.ASCII.GetString(dataReceived, 0, recv));
    }

    private void Start()
    {
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

