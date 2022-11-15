using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player
{
    public Vector3 position;
}

public class JsonSerialization : MonoBehaviour
{
    private MemoryStream stream;
    private Player playerTest = new Player();
    private Player player2Test = new Player();

    private void Awake()
    {
        playerTest.position = Vector3.one;
    }
    private void Update()
    {
        playerTest.position.x += Time.deltaTime;
        JsonSerialize(playerTest);
        JsonDeserialize(player2Test);

        Debug.Log("PlayerSend = " + playerTest.position);
        Debug.Log("PlayerReceive = " + player2Test.position);
    }

    public void JsonSerialize(Player player)
    {
        string json = JsonUtility.ToJson(player);
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(json);
    }

    void JsonDeserialize(Player player)
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        string json = reader.ReadString();
        JsonUtility.FromJsonOverwrite(json, player);
    }
}