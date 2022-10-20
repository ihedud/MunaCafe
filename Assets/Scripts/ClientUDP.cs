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

    int recv;
    byte[] data;
    IPEndPoint ipep;
    Socket newsock;
    IPEndPoint server;
    EndPoint Remote;
    string message;

    Thread myThread;

    private void clientConnection()
    {
        Debug.LogWarning("Starting Thread");

        Debug.Log("Sending Message");

        server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
        Remote = (EndPoint)(server);

        message = "Hi i want to connect";
        data = Encoding.ASCII.GetBytes(message);
        newsock.SendTo(data, data.Length, SocketFlags.None, Remote);

        //recv = newsock.ReceiveFrom(data, ref Remote);
        //Debug.Log(Remote.ToString());


        //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

    }

    public void Start()
    {
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Any, 9050);

        newsock = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram, ProtocolType.Udp);

        myThread = new Thread(clientConnection);
        myThread.Start();

    }

    void onDisable()
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

