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

    private int recv;
    private byte[] data = new byte[1024];
    private IPEndPoint ipep;
    private Socket newsock;
    private IPEndPoint server;
    private EndPoint Remote;
    private string message;

    private Thread myThread;

    private void ClientConnection()
    {
        Debug.LogWarning("Starting Thread");
        Debug.Log("Sending Message");

        server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
        //server = new IPEndPoint(IPAddress.Parse("192.168.1.44"), 9050);
        Remote = (EndPoint)server;

        message = "Hi i want to connect bdaghfevtqeadtfhgbetHGDFRBGRWefsvZ";
        data = Encoding.Default.GetBytes(message);
        recv = newsock.SendTo(data, data.Length, SocketFlags.None, Remote);

        //recv = newsock.ReceiveFrom(data, ref Remote);
        //Debug.Log(Remote.ToString());

        // Montu code
        //try
        //{
            recv = newsock.Receive(data);
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
       // }
        //catch(Exception e)
        //{
            //Debug.Log("[CLIENT] Failed to send:" + e.ToString());
        //}

    }

    private void Start()
    {
        //ipep = new IPEndPoint(IPAddress.Any, 9050);

        // Start the socket
        newsock = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram, ProtocolType.Udp);

        // Start the thread
        myThread = new Thread(ClientConnection);
        myThread.Start();

    }

    private void OnDisable()
    {
        try
        {
            myThread.Abort();

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

}

