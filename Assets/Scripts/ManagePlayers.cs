using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagePlayers : MonoBehaviour
{
    [Serializable]
    public class Player
    {
        public GameObject card;
        [HideInInspector] public string username;
        [HideInInspector] public bool connected = false;
    }

    [SerializeField] public List<Player> players = new List<Player>();

    private bool updatePlayers = false;

    public void ConnectPlayer(string username, int playerNumber)
    {
        updatePlayers = true;

        players[playerNumber].username = username;
        players[playerNumber].connected = true;
    }

    private void Update()
    {
        if (!updatePlayers)
            return;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].connected && !players[i].card.activeInHierarchy)
            {
                players[i].card.SetActive(true);
                players[i].card.transform.Find("Username").GetComponent<TMP_Text>().text = players[i].username;
            }
        }

        updatePlayers = false;
    }
}