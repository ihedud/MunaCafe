using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManagePlayers : MonoBehaviour
{
    [Serializable]
    public class Player
    {
        public GameObject card;
        [HideInInspector] public string username;
        [HideInInspector] public bool connected = false;
        [HideInInspector] public bool displayingEmoji = false;
        [HideInInspector] public int emojiID;
    }

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] public List<Player> players = new List<Player>();

    private bool updatePlayers = false;
    private bool showEmoji = false;

    public void ConnectPlayer(string username, int playerNumber)
    {
        updatePlayers = true;

        players[playerNumber].username = username;
        players[playerNumber].connected = true;
    }

    public void ShowEmoji(string username, int emojiID)
    {
        showEmoji = true;

        FindPlayer(username).displayingEmoji = true;
        FindPlayer(username).emojiID = emojiID;

        UpdateEmoji();
    }

    private Player FindPlayer(string username)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].username == username)
                return players[i];
        }
        return null;
    }

    private void UpdateEmoji()
    {
        if (showEmoji)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].displayingEmoji)
                {
                    players[i].card.transform.Find("Image").GetComponent<Image>().sprite = sprites[players[i].emojiID];
                }
            }
        }
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