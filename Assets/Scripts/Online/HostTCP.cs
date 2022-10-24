using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System.Text;

public class HostTCP : MonoBehaviour
{
    [SerializeField] private int port = 9050;

    private int recv;
    private string message;
    private byte[] dataReceived = new byte[1024];
    private byte[] dataSent = new byte[1024];
    private bool closed = true;

    private IPEndPoint client;
    private Socket newSocket;
    private Socket clientSocket;
    private Thread myThread;

    public void Initialize()
    {
        // Initialize socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Initialize thread
        myThread = new Thread(HostConnection);
        closed = false;
        myThread.Start();
    }

    private void HostConnection()
    {
        client = new IPEndPoint(IPAddress.Any, port);
        newSocket.Bind(client);
        newSocket.Listen(10);

        while (!closed)
        {
            Debug.Log("Starting Thread");
            Debug.Log("Waiting for a client...");

            clientSocket = newSocket.Accept();

            // Receive data
            recv = clientSocket.Receive(dataReceived);
            Debug.Log(Encoding.ASCII.GetString(dataReceived, 0, recv));

            // Send data
            message = "Welcome to my test TCP server!";
            dataSent = Encoding.Default.GetBytes(message);
            clientSocket.Send(dataSent, dataSent.Length, SocketFlags.None);
        }
    }

    private void OnDisable()
    {
        closed = true;

        try
        {
            myThread.Abort();
            newSocket.Close();
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}