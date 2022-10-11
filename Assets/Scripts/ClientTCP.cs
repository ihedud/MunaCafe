using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ClientTCP : MonoBehaviour
{
    Socket newSocket;
    int port;
    // Start is called before the first frame update
    void Start()
    {
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(“192.168.204.31”), port);
        //newSocket.Bind(ipep);
    }

    // Update is called once per frame
    void Update()
    {
        //newSocket.Connect(ipep);
    }
}
