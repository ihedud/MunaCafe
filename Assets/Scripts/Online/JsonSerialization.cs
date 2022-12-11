using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSerialization
{
    public string JsonSerialize(Dictionary<int, ObjectFields> packetToSend)
    {
        string jsonSer = JsonUtility.ToJson(packetToSend);
        return jsonSer;
    }

    public Dictionary<int, ObjectFields> JsonDeserialize(string jsonPacket)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(jsonPacket);
        ReplicationManager repManager = new ReplicationManager();
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        jsonPacket = reader.ReadString();
        JsonUtility.FromJsonOverwrite(jsonPacket, repManager.packetReceived);
        return repManager.packetReceived;
    }
}