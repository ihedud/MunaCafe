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
    private MemoryStream stream;

    [HideInInspector] public string jsonSer;
    [HideInInspector] public string jsonDes;

    public string JsonSerialize(Information info)
    {
        jsonSer = JsonUtility.ToJson(info);
        stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(jsonSer);
        return jsonSer;
    }

    public Information JsonDeserialize(string jsonInfo)
    {
        Information info = new Information();
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        jsonDes = reader.ReadString();
        JsonUtility.FromJsonOverwrite(jsonDes, info);
        return info;
    }
}