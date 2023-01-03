using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] private float maxTime;

    // UI
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject mainLight;

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

        if (currentTime < maxTime)
            currentTime += Time.deltaTime;
        else
            FinishGame();
    }

    private void FinishGame()
    {
        endGame = true;

        inGameUI.SetActive(false);
        mainCamera.SetActive(false);
        mainLight.SetActive(false);

        endGameUI.SetActive(true);
    }
}
