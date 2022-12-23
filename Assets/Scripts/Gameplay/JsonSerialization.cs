using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Information
{
    //// Packet Information
    //public int hostPacketID = 0;
    //public int clientPacketID = 0;

    // Player Information
    public string username = " ";
    public bool onPlay = false;
    public Vector3 playerPos = Vector3.zero;
    public int colorID = 0;
    public bool hasPing = false;
    public bool hasInteracted = false;
    public int order1 = 0;
    public int order2 = 0;
}

public class JsonSerialization : MonoBehaviour
{
    public string JsonSerialize(Information info)
    {
        string jsonSer = JsonUtility.ToJson(info);
        return jsonSer;
    }

    public Information JsonDeserialize(string jsonInfo)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(jsonInfo);
        Information info = new Information();
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        jsonInfo = reader.ReadString();
        JsonUtility.FromJsonOverwrite(jsonInfo, info);
        return info;
    }
}