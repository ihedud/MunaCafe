using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    // Time
    [SerializeField] private float maxGameTime;

    // UI
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject mainLight;
    [SerializeField] private LoadScene loader;

    private float currentTime;

    private bool endGame;

    private void Awake()
    {
        currentTime = 0f;
        endGame = false;
    }


    private void Update()
    {
        if (endGame)
            return;

        if (currentTime < maxGameTime)
            currentTime += Time.deltaTime;
        else
            ShowScore();
    }

    private void ShowScore()
    {
        endGame = true;

        inGameUI.SetActive(false);
        mainCamera.SetActive(false);
        mainLight.SetActive(false);

        endGameUI.SetActive(true);
    }
}