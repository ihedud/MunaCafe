using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeaCup : MonoBehaviour
{
    // Input
    [SerializeField] private InputActionReference playerGrab;

    private GameObject player;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = collider.gameObject;
            playerGrab.action.Enable();
            playerGrab.action.performed += GrabCup;
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
            playerGrab.action.performed -= GrabCup;
            player.GetComponent<PlayerState>().hasInteracted = false;
        }
    }

    private void GrabCup(InputAction.CallbackContext context)
    {
        player.GetComponent<PlayerState>().hasInteracted = true;
        TeaInteraction();
    }

    private void TeaInteraction()
    {
        if (player.GetComponent<PlayerState>().currentState == PlayerState.State.None)
        {
            player.GetComponent<PlayerState>().currentState = PlayerState.State.EmptyTea;
            StartCoroutine(Grabbing());
        }
    }

    private IEnumerator Grabbing()
    {
        playerGrab.action.Disable();
        playerGrab.action.performed -= GrabCup;

        yield return new WaitForSeconds(0.5f);

        player.GetComponent<PlayerState>().hasInteracted = false;
        gameObject.SetActive(false);
    }
}
