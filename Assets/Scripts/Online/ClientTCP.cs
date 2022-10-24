using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System;
using System.Net;
using System.Text;
using TMPro;

public class ClientTCP : MonoBehaviour
{
    [SerializeField] private GameObject serverNameInputField;
    [SerializeField] private int port = 9050;

    private string ipAddress;

    private int recv;
    private string message;
    private byte[] dataSent = new byte[1024];
    private byte[] dataReceived = new byte[1024];

    private IPEndPoint host;
    private Socket newSocket;
    private Thread myThread;

    public void Initialize()
    {
        // Get data from session
        ipAddress = serverNameInputField.GetComponent<TMP_InputField>().text;

        // Initialize socket
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Initialize thread
        myThread = new Thread(ClientConnection);
        myThread.Start();
    }

    private void ClientConnection()
    {
        Debug.LogWarning("Starting Thread");
        Debug.Log("Sending Message");

        host = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        newSocket.Connect(host);

        // Send data
        message = "Hi, I want to connect!";
        dataSent = Encoding.Default.GetBytes(message);
        newSocket.Send(dataSent, dataSent.Length, SocketFlags.None);

        // Receive data
        recv = newSocket.Receive(dataReceived);
        Debug.Log(Encoding.ASCII.GetString(dataReceived, 0, recv));
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