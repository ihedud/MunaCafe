using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;

public class HostUDP : MonoBehaviour
{
    [SerializeField] private int port = 9050;

    private int recv;
    private string message;
    private byte[] dataSent = new byte[1024];
    private byte[] dataReceived = new byte[1024];
    private bool closed = true;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;

    private void HostConnection()
    {
        client = new IPEndPoint(IPAddress.Any, port);
        remote = (EndPoint)client;
        newSocket.Bind(client);

        while (!closed)
        {
            Debug.Log("Starting Thread");
            Debug.Log("Waiting for a client...");

            // Receive Data
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            Debug.Log(remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(dataReceived, 0, recv));

            // Send Data
            message = "Welcome to my test UDP server!";
            dataSent = Encoding.ASCII.GetBytes(message);
            newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);
        }
    }

    private void Start()
    {
        // Initialize Socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Initialize Thread
        myThread = new Thread(HostConnection);
        closed = false;
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
