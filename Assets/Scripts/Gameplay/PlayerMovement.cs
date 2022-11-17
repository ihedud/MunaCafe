using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Information playerTemp;
    JsonSerialization jsonTemp;

    private void Awake()
    {
        playerTemp = new Information();
        jsonTemp = new JsonSerialization();
    }

    private void FixedUpdate()
    {
        //jsonTemp.JsonDeserialize(playerTemp);
        Debug.Log(playerTemp.playerPos);
        transform.position = new Vector3(playerTemp.playerPos.x + 10, playerTemp.playerPos.y, playerTemp.playerPos.z);
    }
}
