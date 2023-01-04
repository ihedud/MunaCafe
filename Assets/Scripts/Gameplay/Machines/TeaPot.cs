using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeaPot : MonoBehaviour
{
    enum State { Empty, Brewing, Done, Burned, Cooldown };

    [SerializeField] private int brewingTime;
    [SerializeField] private int burningTime;
    [SerializeField] private int cooldownTime;

    // State
    [SerializeField] private GameObject sphere;
    [SerializeField] private Material red;
    [SerializeField] private Material orange;
    [SerializeField] private Material green;
    [SerializeField] private Material grey;
    [SerializeField] private Material black;

    // Input
    [SerializeField] private InputActionReference playerGrab;

    [SerializeField] private GameObject cup;

    private GameObject player;
    private State currentState = State.Empty;

    private void Awake()
    {
        currentState = State.Empty;
        sphere.GetComponent<MeshRenderer>().material = red;
        cup.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = collider.gameObject;
            playerGrab.action.Enable();
            playerGrab.action.performed += GrabTea;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player2" && collider.gameObject.GetComponent<PlayerState>().hasInteracted && !collider.gameObject.GetComponent<PlayerState>().interactionDone)
        {
            player = collider.gameObject;
            TeaInteraction();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerGrab.action.Disable();
            playerGrab.action.performed -= GrabTea;
            player.GetComponent<PlayerState>().hasInteracted = false;
        }
    }

    private void GrabTea(InputAction.CallbackContext context)
    {
        player.GetComponent<PlayerState>().hasInteracted = true;
        TeaInteraction();
    }

    private void TeaInteraction()
    {
        if (currentState == State.Empty)
            StartCoroutine(Brewing());

        if (currentState == State.Done && player.GetComponent<PlayerState>().currentState == PlayerState.State.EmptyTea)
        {
            cup.SetActive(false);

            player.GetComponent<PlayerState>().currentState = PlayerState.State.Tea;

            StartCoroutine(Cooldown());
        }
        else if (currentState == State.Burned && player.GetComponent<PlayerState>().currentState == PlayerState.State.EmptyTea)
        {
            cup.SetActive(false);

            player.GetComponent<PlayerState>().currentState = PlayerState.State.BurnedTea;

            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        currentState = State.Cooldown;
        sphere.GetComponent<MeshRenderer>().material = grey;

        yield return new WaitForSeconds(cooldownTime);

        currentState = State.Empty;
        sphere.GetComponent<MeshRenderer>().material = red;

        player.GetComponent<PlayerState>().hasInteracted = false;
    }

    private IEnumerator Brewing()
    {
        cup.SetActive(true);
        currentState = State.Brewing;
        sphere.GetComponent<MeshRenderer>().material = orange;

        yield return new WaitForSeconds(brewingTime);

        currentState = State.Done;
        sphere.GetComponent<MeshRenderer>().material = green;

        yield return new WaitForSeconds(burningTime);

        if (currentState == State.Done)
        {
            currentState = State.Burned;
            sphere.GetComponent<MeshRenderer>().material = black;
        }

        player.GetComponent<PlayerState>().hasInteracted = false;
    }
}

