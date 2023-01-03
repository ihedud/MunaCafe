using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [Serializable]
    public class PlayerEnd
    {
        public GameObject card;
        [HideInInspector] public string username;
        [HideInInspector] public bool connected = false;
    }

    //[SerializeField] private Host 
    //[SerializeField] public List<PlayerEnd> players = new List<PlayerEnd>();

    //public void Awake()
    //{
    //    players[playerNumber].username = username;


    //}
}
