using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoffeeMachine : MonoBehaviour
{
    enum State { Empty, Brewing, Done };

    [SerializeField] private int brewingTime;

    // State
    [SerializeField] private GameObject sphere;
    [SerializeField] private Material red;
    [SerializeField] private Material orange;
    [SerializeField] private Material green;

    // Input
    [SerializeField] private InputActionReference playerGrab;

    [SerializeField] private GameObject coffee;
    [SerializeField] private GameObject mug;

    private GameObject player;
    private State currentState = State.Empty;

    private void Awake()
    {
        currentState = State.Empty;
        sphere.GetComponent<MeshRenderer>().material = red;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = collider.gameObject;
            playerGrab.action.Enable();
            playerGrab.action.performed += GrabCoffee;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player2" && collider.gameObject.GetComponent<PlayerState>().hasInteracted && !collider.gameObject.GetComponent<PlayerState>().interactionDone)
        {
            player = collider.gameObject;
            CoffeeInteraction();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerGrab.action.Disable();
            playerGrab.action.performed -= GrabCoffee;
            player.GetComponent<PlayerState>().hasInteracted = false;
        }
    }

    private void GrabCoffee(InputAction.CallbackContext context)
    {
        player.GetComponent<PlayerState>().hasInteracted = true;
        CoffeeInteraction();
    }

    private void CoffeeInteraction()
    {
        if (currentState == State.Empty)
            StartCoroutine(Brewing());

        if (currentState == State.Done && player.GetComponent<PlayerState>().currentState == PlayerState.State.None)
        {
            coffee.SetActive(false);
            mug.SetActive(false);

            player.GetComponent<PlayerState>().currentState = PlayerState.State.Coffee;

            currentState = State.Empty;
            sphere.GetComponent<MeshRenderer>().material = red;
        }

        player.GetComponent<PlayerState>().hasInteracted = false;
    }

    private IEnumerator Brewing()
    {
        mug.SetActive(true);
        currentState = State.Brewing;
        sphere.GetComponent<MeshRenderer>().material = orange;

        yield return new WaitForSeconds(brewingTime);

        currentState = State.Done;
        sphere.GetComponent<MeshRenderer>().material = green;

        coffee.SetActive(true);
    }
}
