using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrayClient : MonoBehaviour
{
    [HideInInspector] public enum Order { Coffee, Tea, Donut };

    [HideInInspector] public Order currentOrder;

    private enum TrayState { Empty, Ongoing, Completed };
    private TrayState currentTrayState;

    [SerializeField] private GameObject coffeeTR;
    [SerializeField] private GameObject coffee;
    [SerializeField] private GameObject teaTR;
    [SerializeField] private GameObject tea;
    [SerializeField] private GameObject donutTR;
    [SerializeField] private GameObject donut;

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
            case Order.Tea:
                teaTR.SetActive(true);
                break;
            case Order.Donut:
                donutTR.SetActive(true);
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
                    if (currentOrder == Order.Coffee)
                    {
                        currentTrayState = TrayState.Completed;
                        coffeeTR.SetActive(false);
                        coffee.SetActive(true);
                        pointsManager.UpdatePoints(1, player);
                        StartCoroutine(CompleteOrder());
                    }
                    break;
                case PlayerState.State.Tea:
                    if (currentOrder == Order.Tea)
                    {
                        currentTrayState = TrayState.Completed;
                        teaTR.SetActive(false);
                        tea.SetActive(true);
                        pointsManager.UpdatePoints(1, player);
                        StartCoroutine(CompleteOrder());
                    }
                    break;
                case PlayerState.State.Donut:
                    if (currentOrder == Order.Donut)
                    {
                        currentTrayState = TrayState.Completed;
                        donutTR.SetActive(false);
                        donut.SetActive(true);
                        pointsManager.UpdatePoints(2, player);
                        StartCoroutine(CompleteOrder());
                    }
                    break;
            }
        }
    }

    private IEnumerator CompleteOrder()
    {
        player.GetComponent<PlayerState>().currentState = PlayerState.State.None;

        yield return new WaitForSeconds(1f);

        coffee.SetActive(false);
        tea.SetActive(false);
        donut.SetActive(false);

        currentTrayState = TrayState.Empty;
    }
}
