using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Host : MonoBehaviour
{
    Socket newSocket;
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
        
    }

    // https://forum.unity.com/threads/tcp-sockets-how-to-receive-from-client.541502/

}
