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

    int recv;
    byte[] data;
    IPEndPoint ipep;
    Socket newsock;
    IPEndPoint sender;
    EndPoint Remote;
    //string welcome;

    bool closed = true;
    Thread myThread;

    private void hostConnection()
    {
        //while to keep waiting for messages
        while(!closed)
        {
            Debug.LogWarning("Starting Thread");

            Debug.Log("Waiting for a client...");

            sender = new IPEndPoint(IPAddress.Any, 0);
            Remote = (EndPoint)(sender);

            recv = newsock.ReceiveFrom(data, ref Remote);

            Debug.Log(Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            //welcome = "Welcome to my test server";
            //data = Encoding.ASCII.GetBytes(welcome);
            //newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
        }


    }

    public void Start()
    {
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Any, 9050);

        newsock = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram, ProtocolType.Udp);

        newsock.Bind(ipep);

        Thread myThread = new Thread(hostConnection);
        closed = false;
        myThread.Start();

    }

    void OnDisable()
    {

        closed = true;

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
