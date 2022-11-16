using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Player
{
    public Vector3 position;
}

public class JsonSerialization : MonoBehaviour
{
    private MemoryStream stream;

    [HideInInspector] public string jsonSer;
    [HideInInspector] public string jsonDes;

    public void JsonSerialize(Player player)
    {
        jsonSer = JsonUtility.ToJson(player);
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(jsonSer);
    }

    public void JsonDeserialize(Player player)
    {
        if (jsonSer != null)
        {
            BinaryReader reader = new BinaryReader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            jsonDes = reader.ReadString();
            JsonUtility.FromJsonOverwrite(jsonDes, player);
        }
    }
}