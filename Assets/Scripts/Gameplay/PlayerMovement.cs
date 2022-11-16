using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Player playerTemp;
    JsonSerialization jsonTemp;

    private void Awake()
    {
        playerTemp = new Player();
        jsonTemp = new JsonSerialization();
    }

    private void FixedUpdate()
    {
        jsonTemp.JsonDeserialize(playerTemp);
        Debug.Log(playerTemp.position);
        transform.position = new Vector3(playerTemp.position.x + 10, playerTemp.position.y, playerTemp.position.z);
    }
}
