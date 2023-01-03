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
    }

    [SerializeField] private Host host;
    [SerializeField] private Client client;

    [SerializeField] private TextMeshProUGUI username1;
    [SerializeField] private TextMeshProUGUI username2;
    [SerializeField] private MeshRenderer capsule1;
    [SerializeField] private MeshRenderer capsule2;
    [SerializeField] private MeshRenderer player1Mat;
    [SerializeField] private MeshRenderer player2Mat;

    private Information player1;
    private Information player2;

    private void Awake()
    {
        if (host != null)
        {
            player1 = host.hostInfo.myInfo;
            player2 = host.hostInfo.clientInfo;
        }
        else if (client != null)
        {
            player1 = client.clientInfo.hostInfo;
            player2 = client.clientInfo.myInfo;
        }

        username1.text = player1.username;
        username2.text = player2.username;
        capsule1.material = player1Mat.material;
        capsule2.material = player2Mat.material;
    }

}
