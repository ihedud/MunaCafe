using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrashCan : MonoBehaviour
{
    [SerializeField] private GameObject cup;

    // Input
    [SerializeField] private InputActionReference playerGrab;

    private GameObject player;

    private void Awake()
    {
        playerGrab.action.Enable();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = collider.gameObject;
            playerGrab.action.performed += ThrowTrash;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player2" && collider.gameObject.GetComponent<PlayerState>().hasInteracted && !collider.gameObject.GetComponent<PlayerState>().interactionDone)
        {
            player = collider.gameObject;
            TrashInteraction();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerGrab.action.performed -= ThrowTrash;
            player.GetComponent<PlayerState>().hasInteracted = false;
        }
    }

    private void ThrowTrash(InputAction.CallbackContext context)
    {
        player.GetComponent<PlayerState>().hasInteracted = true;
        TrashInteraction();
    }

    private void TrashInteraction()
    {
        if (player.GetComponent<PlayerState>().currentState == PlayerState.State.EmptyTea)
            cup.SetActive(true);
        if (player.GetComponent<PlayerState>().currentState != PlayerState.State.None)
            player.GetComponent<PlayerState>().currentState = PlayerState.State.None;
    }

    private void OnDisable()
    {
        playerGrab.action.Disable();
    }
}
