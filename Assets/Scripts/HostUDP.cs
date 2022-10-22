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
    private int recv;
    private byte[] data = new byte[1024];
    private IPEndPoint ipep;
    private Socket newsock;
    //private IPEndPoint sender;
    private EndPoint Remote;
    private string welcome;

    private bool closed = true;
    private Thread myThread;

    private void HostConnection()
    {
        //while to keep waiting for messages
        while(!closed)
        {
            Debug.LogWarning("Starting Thread");

            Debug.Log("Waiting for a client...");

            //sender = new IPEndPoint(IPAddress.Any, 0);
            ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

            Remote = (EndPoint)ipep;

            newsock.Bind(ipep);
            recv = newsock.ReceiveFrom(data, ref Remote);

            Debug.Log(Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));


            // Send Data
            welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
        }


    }

    public void Start()
    {

        // Socket
        newsock = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram, ProtocolType.Udp);

        // Thread
        myThread = new Thread(HostConnection);
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
