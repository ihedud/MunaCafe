using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tray : MonoBehaviour
{
    private enum Order { Coffee };

    private Order currentOrder;

    private enum TrayState { Empty, Ongoing, Completed };
    private TrayState currentTrayState;

    [SerializeField] private GameObject coffeeTR;
    [SerializeField] private GameObject coffee;

    private GameObject player;

    // Input
    [SerializeField] private InputActionReference playerGrab;

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

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerGrab.action.Disable();
            playerGrab.action.performed -= DeliverOrder;
        }
    }

    private void Update()
    {
        if(currentTrayState == TrayState.Empty)
        {
            StartCoroutine(AssignOrder());
        }
    }

    private IEnumerator AssignOrder()
    {
        currentTrayState = TrayState.Ongoing;

        yield return new WaitForSeconds(Random.Range(4, 8));

        currentOrder = (Order)Random.Range(0, 0);

        switch(currentOrder)
        {
            case Order.Coffee:
                coffeeTR.SetActive(true);
                break;
        }
    }

    private void DeliverOrder(InputAction.CallbackContext context)
    {
        if (currentTrayState == TrayState.Ongoing)
        {
            currentTrayState = TrayState.Completed;
            switch (player.GetComponent<PlayerState>().currentState)
            {
                case PlayerState.State.Coffee:
                    coffeeTR.SetActive(false);
                    coffee.SetActive(true);
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