using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    [HideInInspector] public int points = 0;
    private int player1Points = 0;
    private int player2Points = 0;

    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = points.ToString();
    }

    public void UpdatePoints(int points, GameObject player)
    {
        if (player.tag == "Player")
            player1Points += points;

        else if (player.tag == "Player2")
            player2Points += points;

        points = player1Points + player2Points;

        text.text = points.ToString();
    }
}
