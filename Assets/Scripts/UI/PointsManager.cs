using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    [HideInInspector] public int totalPoints = 0;
    [HideInInspector] public int player1Points = 0;
    [HideInInspector] public int player2Points = 0;

    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        text.text = totalPoints.ToString();
    }

    public void UpdatePoints(int points, GameObject player)
    {
        if (player.name == "Player1")
            player1Points += points;

        else if (player.name == "Player2")
            player2Points += points;

        totalPoints = player1Points + player2Points;

        text.text = totalPoints.ToString();
    }
}
