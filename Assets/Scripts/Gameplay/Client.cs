using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    private ClientUDP client;
    //Information playerTemp;

    private void Awake()
    {
        client = FindObjectOfType<ClientUDP>();
        //playerTemp = new Information();
    }

    private void FixedUpdate()
    {
        if (client != null)
        {
            client.myInfo.playerPos = transform.position;
        }
        //Debug.Log(playerTemp.playerPos);
        //transform.position = new Vector3(playerTemp.playerPos.x + 10, playerTemp.playerPos.y, playerTemp.playerPos.z);
    }
}
