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
    [SerializeField] private TextMeshProUGUI points1;
    [SerializeField] private TextMeshProUGUI points2;

    [SerializeField] private float scoreDisplayedTime;

    [SerializeField] private PointsManager pointsManager;
    [SerializeField] private LoadScene loader;

    private Information player1;
    private Information player2;

    private void OnEnable()
    {
        points1.text = pointsManager.player1Points.ToString();
        points2.text = pointsManager.player2Points.ToString();

        StartCoroutine(LeaveToLobby());
    }

    private void Start()
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

    private IEnumerator LeaveToLobby()
    {
        yield return new WaitForSeconds(scoreDisplayedTime);

        // Esto es si vamos al lobby
        // Tenemos que hacer que al cargar estas escenas,
        // salga bien el canvas que toca (lobby)

        //if (host != null)
        //    loader.LoadNextScene("UDP_NewGame");
        //else if (client != null)
        //    loader.LoadNextScene("UDP_JoinGame");

        if (host != null)
            Destroy(FindObjectOfType<HostUDP>());
        else if (client != null)
            Destroy(FindObjectOfType<ClientUDP>());

        loader.LoadNextScene("MainMenu");
    }

}
