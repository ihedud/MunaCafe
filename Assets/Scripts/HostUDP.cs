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

    private string serverName;
    private string username;

    private int recv;
    private byte[] dataSent = new byte[1024];
    private byte[] dataReceived = new byte[1024];
    private bool closed = true;

    private IPEndPoint client;
    private EndPoint remote;
    private Socket newSocket;
    private Thread myThread;

    private void HostConnection()
    {
        Debug.Log("Starting Thread");

        client = new IPEndPoint(IPAddress.Any, port);
        remote = (EndPoint)client;
        newSocket.Bind(client);

        while (!closed)
        {
            Debug.Log("Waiting for clients...");

            // Receive Data
            recv = newSocket.ReceiveFrom(dataReceived, ref remote);
            Debug.Log(remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(dataReceived, 0, recv));

            // Send Data
            dataSent = Encoding.ASCII.GetBytes(serverName);
            newSocket.SendTo(dataSent, dataSent.Length, SocketFlags.None, remote);
        }
    }

    public void Initializing()
    {
        // Get Data From Session
        serverName = serverNameInputField.GetComponent<TMP_InputField>().text;
        username = usernameInputField.GetComponent<TMP_InputField>().text;

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
