using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Emoji : MonoBehaviour
{
    [SerializeField] private ManagePlayers playerManager;
    [SerializeField] private GameObject usernameInputField;
    [SerializeField] private int id;

    public void SendEmoji()
    {
        for (int i = 0; i < playerManager.players.Count; i++)
        {
            if (playerManager.players[i].username == usernameInputField.GetComponent<TMP_InputField>().text)
            {
                playerManager.players[i].emojiID = id;
                playerManager.ShowEmoji(playerManager.players[i].username, id);
            }
        }

    }
}
