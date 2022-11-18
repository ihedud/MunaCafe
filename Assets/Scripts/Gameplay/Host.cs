using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    private HostUDP host;
    //Information playerTemp;

    private void Awake()
    {
        host = FindObjectOfType<HostUDP>();
        //playerTemp = new Information();
    }

    private void FixedUpdate()
    {
        if (host != null)
        {
             transform.position = host.clientInfo.playerPos;
        }
        //Debug.Log(playerTemp.playerPos);
        //transform.position = new Vector3(playerTemp.playerPos.x + 10, playerTemp.playerPos.y, playerTemp.playerPos.z);
    }
}
