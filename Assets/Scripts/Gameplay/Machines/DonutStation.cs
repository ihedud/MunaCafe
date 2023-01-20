using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DonutStation : MonoBehaviour
{
    public enum State { Empty, Start, Half, Finished };

    [SerializeField] private int updateTime;

    // State
    [SerializeField] private GameObject sphere;
    [SerializeField] private GameObject sphereTR;
    [SerializeField] private Material red;
    [SerializeField] private Material orange;
    [SerializeField] private Material yellow;
    [SerializeField] private Material green;

    // Input
    [SerializeField] private InputActionReference playerGrab;

    [SerializeField] private GameObject start;
    [SerializeField] private GameObject half;
    [SerializeField] private GameObject finished;

    private GameObject player;
    public State currentState = State.Empty;
    private bool isBaking;
    private float timer;
    private Vector3 initScale;

    private void Awake()
    {
        currentState = State.Empty;
        sphere.GetComponent<MeshRenderer>().material = red;
        initScale = sphere.transform.localScale;
        start.SetActive(false);
        half.SetActive(false);
        finished.SetActive(false);
        sphereTR.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = collider.gameObject;
            playerGrab.action.Enable();
            playerGrab.action.performed += GrabDonut;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player2" && collider.gameObject.GetComponent<PlayerState>().hasInteracted && !collider.gameObject.GetComponent<PlayerState>().interactionDone)
        {
            player = collider.gameObject;
            DonutInteraction();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Player2")
        {
            playerGrab.action.Disable();
            playerGrab.action.performed -= GrabDonut;
            player.GetComponent<PlayerState>().hasInteracted = false;
            if (currentState != State.Finished)
                Restart();
        }
    }

    private void GrabDonut(InputAction.CallbackContext context)
    {
        player.GetComponent<PlayerState>().hasInteracted = true;
        DonutInteraction();
    }

    private void DonutInteraction()
    {
        if (currentState == State.Empty)
        {
            isBaking = true;
            currentState = State.Start;
            start.SetActive(true);
            half.SetActive(false);
            finished.SetActive(false);
            sphere.GetComponent<MeshRenderer>().material = yellow;
            sphereTR.SetActive(true);
            timer = 0.0f;
        }
        else if (currentState == State.Finished && player.GetComponent<PlayerState>().currentState == PlayerState.State.None)
        {
            player.GetComponent<PlayerState>().currentState = PlayerState.State.Donut;
            Restart();
        }
    }

    private void Update()
    {
        if (!isBaking)
            return;

        timer += Time.deltaTime;
        if(currentState != State.Finished)
            sphere.transform.localScale += new Vector3(0.000009f, 0.000009f, 0.000009f);

        if (timer > updateTime)
        {
            switch(currentState)
            {
                case State.Start:
                    currentState = State.Half;
                    sphere.GetComponent<MeshRenderer>().material = orange;
                    start.SetActive(false);
                    finished.SetActive(false);
                    half.SetActive(true);
                    break;
                case State.Half:
                    currentState = State.Finished;
                    sphere.GetComponent<MeshRenderer>().material = green;
                    half.SetActive(false);
                    start.SetActive(false);
                    finished.SetActive(true);
                    sphereTR.SetActive(false);
                    player.GetComponent<PlayerState>().hasInteracted = false;
                    break;
            }
            timer = 0.0f;
        }
    }

    private void Restart()
    {
        timer = 0.0f;
        isBaking = false;
        currentState = State.Empty;
        start.SetActive(false);
        half.SetActive(false);
        finished.SetActive(false);
        sphereTR.SetActive(false);
        sphere.GetComponent<MeshRenderer>().material = red;
        sphere.transform.localScale = initScale;
    }
}
