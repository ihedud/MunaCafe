using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrayClient : MonoBehaviour
{
    [HideInInspector] public enum Order { Coffee };

    [HideInInspector] public Order currentOrder;

    private enum TrayState { Empty, Ongoing, Completed };
    private TrayState currentTrayState;

    [SerializeField] private GameObject coffeeTR;
    [SerializeField] private GameObject coffee;

    private GameObject player;

    // Input
    [SerializeField] private InputActionReference playerGrab;

    [SerializeField] private PointsManager pointsManager;

    private void Awake()
    {
        currentTrayState = TrayState.Empty;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = collider.gameObject;
            playerGrab.action.Enable();
            playerGrab.action.performed += DeliverOrder;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player2" && collider.gameObject.GetComponent<PlayerState>().hasInteracted && !collider.gameObject.GetComponent<PlayerState>().interactionDone)
        {
            player = collider.gameObject;
            TrayInteraction();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerGrab.action.Disable();
            playerGrab.action.performed -= DeliverOrder;
            player.GetComponent<PlayerState>().hasInteracted = false;
        }
    }

    private void Update()
    {
        if (currentTrayState == TrayState.Empty)
        {
            StartCoroutine(AssignOrder());
        }
    }

    private IEnumerator AssignOrder()
    {
        currentTrayState = TrayState.Ongoing;

        yield return new WaitForSeconds(6f);

        switch (currentOrder)
        {
            case Order.Coffee:
                coffeeTR.SetActive(true);
                break;
        }
    }

    private void DeliverOrder(InputAction.CallbackContext context)
    {
        player.GetComponent<PlayerState>().hasInteracted = true;
        TrayInteraction();
    }

    private void TrayInteraction()
    {
        if (currentTrayState == TrayState.Ongoing)
        {
            switch (player.GetComponent<PlayerState>().currentState)
            {
                case PlayerState.State.Coffee:
                    currentTrayState = TrayState.Completed;
                    coffeeTR.SetActive(false);
                    coffee.SetActive(true);
                    pointsManager.UpdatePoints(1, player);
                    StartCoroutine(CompleteOrder());
                    break;
            }
        }
    }

    private IEnumerator CompleteOrder()
    {
        player.GetComponent<PlayerState>().currentState = PlayerState.State.None;

        yield return new WaitForSeconds(1f);

        coffee.SetActive(false);
        currentTrayState = TrayState.Empty;
    }
}
