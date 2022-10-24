using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System;
using System.Net;
using System.Text;

public class ClientTCP : MonoBehaviour
{
    [SerializeField] private string ipAddress;
    [SerializeField] private int port = 9050;

    private int recv;
    private string message;
    private byte[] dataSent = new byte[1024];
    private byte[] dataReceived = new byte[1024];

    private IPEndPoint host;
    private Socket newSocket;
    private Thread myThread;

    private void ClientConnection()
    {
        Debug.LogWarning("Starting Thread");
        Debug.Log("Sending Message");

        host = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        newSocket.Connect(host);

        // Send Data
        message = "Hi, I want to connect!";
        dataSent = Encoding.Default.GetBytes(message);
        newSocket.Send(dataSent, dataSent.Length, SocketFlags.None);

        // Receive Data
        recv = newSocket.Receive(dataReceived);
        Debug.Log(Encoding.ASCII.GetString(dataReceived, 0, recv));
    }

    private void Start()
    {
        // Initialize Socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
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
