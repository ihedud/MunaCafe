using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ReplicationManager
{
   /*[SerializeField]*/public Dictionary<int, ObjectFields> packetToSend = new Dictionary<int, ObjectFields>();
   /*[SerializeField]*/public Dictionary<int, ObjectFields> packetReceived = new Dictionary<int, ObjectFields>();

    public Dictionary<int, ObjectFields> PacketCreation(int netID, Player player)
    {
        packetToSend.Clear();
        switch(netID)
        {
            case 0:
                PlayerLobby playerLobby = new PlayerLobby();
                playerLobby.username = player.username;
                playerLobby.colorID = player.colorID;
                playerLobby.onPlay = player.onPlay;
                packetToSend.Add(netID, playerLobby);
                break;
            case 1:
                PlayerUpdate playerUpdate = new PlayerUpdate();
                playerUpdate.playerPos = player.playerPos;
                playerUpdate.hasPing = player.hasPing;
                packetToSend.Add(netID, playerUpdate);
                break;
        }

        return packetToSend;
    }

    public Player PacketBreakdown(Dictionary<int, ObjectFields> packet)
    {
        Player player = new Player();
        for(int i = 0; i < packet.Count; i++)
        {
            var item = packet.ElementAt(i);
            switch (item.Key)
            {
                case 0:
                    PlayerLobby playerLobby = (PlayerLobby)item.Value;
                    player.username = playerLobby.username;
                    player.colorID = playerLobby.colorID;
                    player.onPlay = playerLobby.onPlay;
                    break;
                case 1:
                    PlayerUpdate playerUpdate = (PlayerUpdate)item.Value;
                    player.playerPos = playerUpdate.playerPos;
                    player.hasPing = playerUpdate.hasPing;
                    break;
            }
        }
        return player;
    }
}

public class ObjectFields {}

public class PlayerLobby : ObjectFields
{
    public string username = " ";
    public int colorID = 0;
    public bool onPlay = false;
}

public class PlayerUpdate : ObjectFields
{
    public Vector3 playerPos = Vector3.zero;
    public bool hasPing = false;
}

public class Player
{
    public enum Type
    {
        lobby,
        update
    }

    public Type currentType = 0;
    public string username = " ";
    public int colorID = 0;
    public bool onPlay = false;
    public Vector3 playerPos = Vector3.zero;
    public bool hasPing = false;
}