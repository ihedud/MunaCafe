using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Information
{
    public string username = " ";
    public bool onPlay = false;
    public Vector3 playerPos = Vector3.zero;
}

public class JsonSerialization : MonoBehaviour
{
    public string JsonSerialize(Information info)
    {
        string jsonSer = JsonUtility.ToJson(info);
        //
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(jsonSer);
        //
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