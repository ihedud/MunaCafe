using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrayClient : MonoBehaviour
{
    [HideInInspector] public enum Order { Coffee, Tea, Donut, Coffee_Donut, Tea_Donut };

    [HideInInspector] public Order currentOrder;

    public enum TrayState { Empty, Ongoing, Completed, Broken };
    public TrayState currentTrayState;

    public TrayState newState = TrayState.Empty;

    [SerializeField] private GameObject coffeeTR;
    [SerializeField] private GameObject coffee;
    [SerializeField] private GameObject teaTR;
    [SerializeField] private GameObject tea;
    [SerializeField] private GameObject donutTR;
    [SerializeField] private GameObject donut;

    [SerializeField] private GameObject coffeeTR_donutTR;
    [SerializeField] private GameObject coffee_donutTR;
    [SerializeField] private GameObject coffeeTR_donut;
    [SerializeField] private GameObject coffee_donut;

    [SerializeField] private GameObject teaTR_donutTR;
    [SerializeField] private GameObject tea_donutTR;
    [SerializeField] private GameObject teaTR_donut;
    [SerializeField] private GameObject tea_donut;

    [SerializeField] private PlayerState player1;
    [SerializeField] private PlayerState player2;

    [SerializeField] private MeshRenderer trayMesh;
    [SerializeField] private Material grey;

    private GameObject player;
    private int counter = 0;

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

        if (currentTrayState == newState && currentTrayState != TrayState.Broken)
            return;

        counter++;
        if (counter > 300)
        {
            Debug.Log("It broke lol");
            Debug.Log("My state: " + currentTrayState + ", new state = " + newState);

            player1.currentState = PlayerState.State.None;
            player2.currentState = PlayerState.State.None;

            counter = 0;
            StopAllCoroutines();
            StartCoroutine(CompleteOrder());
            currentTrayState = TrayState.Broken;

            trayMesh.material = grey;
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
            case Order.Coffee_Donut:
                coffeeTR_donutTR.SetActive(true);
                break;
            case Order.Tea_Donut:
                teaTR_donutTR.SetActive(true);
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
                    else if (currentOrder == Order.Coffee_Donut)
                    {
                        if (coffeeTR_donutTR.activeSelf)
                        {
                            player.GetComponent<PlayerState>().currentState = PlayerState.State.None;
                            coffeeTR_donutTR.SetActive(false);
                            coffee_donutTR.SetActive(true);
                        }
                        else if (coffeeTR_donut.activeSelf)
                        {
                            coffeeTR_donut.SetActive(false);
                            coffee_donut.SetActive(true);
                            currentTrayState = TrayState.Completed;
                            pointsManager.UpdatePoints(5, player);
                            StartCoroutine(CompleteOrder());
                        }
                    }
                    break;
                case PlayerState.State.Tea:
                    if (currentOrder == Order.Tea)
                    {
                        currentTrayState = TrayState.Completed;
                        teaTR.SetActive(false);
                        tea.SetActive(true);
                        pointsManager.UpdatePoints(2, player);
                        StartCoroutine(CompleteOrder());
                    }
                    else if (currentOrder == Order.Tea_Donut)
                    {
                        if (teaTR_donutTR.activeSelf)
                        {
                            player.GetComponent<PlayerState>().currentState = PlayerState.State.None;
                            teaTR_donutTR.SetActive(false);
                            tea_donutTR.SetActive(true);
                        }
                        else if (teaTR_donut.activeSelf)
                        {
                            teaTR_donut.SetActive(false);
                            tea_donut.SetActive(true);
                            currentTrayState = TrayState.Completed;
                            pointsManager.UpdatePoints(6, player);
                            StartCoroutine(CompleteOrder());
                        }
                    }
                    break;
                case PlayerState.State.Donut:
                    if (currentOrder == Order.Donut)
                    {
                        currentTrayState = TrayState.Completed;
                        donutTR.SetActive(false);
                        donut.SetActive(true);
                        pointsManager.UpdatePoints(3, player);
                        StartCoroutine(CompleteOrder());
                    }
                    else if (currentOrder == Order.Coffee_Donut)
                    {
                        if (coffeeTR_donutTR.activeSelf)
                        {
                            player.GetComponent<PlayerState>().currentState = PlayerState.State.None;
                            coffeeTR_donutTR.SetActive(false);
                            coffeeTR_donut.SetActive(true);
                        }
                        else if (coffee_donutTR.activeSelf)
                        {
                            coffee_donutTR.SetActive(false);
                            coffee_donut.SetActive(true);
                            currentTrayState = TrayState.Completed;
                            pointsManager.UpdatePoints(5, player);
                            StartCoroutine(CompleteOrder());
                        }
                    }
                    else if (currentOrder == Order.Tea_Donut)
                    {
                        if (teaTR_donutTR.activeSelf)
                        {
                            player.GetComponent<PlayerState>().currentState = PlayerState.State.None;
                            teaTR_donutTR.SetActive(false);
                            teaTR_donut.SetActive(true);
                        }
                        else if (tea_donutTR.activeSelf)
                        {
                            tea_donutTR.SetActive(false);
                            tea_donut.SetActive(true);
                            currentTrayState = TrayState.Completed;
                            pointsManager.UpdatePoints(6, player);
                            StartCoroutine(CompleteOrder());
                        }
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
        coffee_donut.SetActive(false);
        tea_donut.SetActive(false);

        currentTrayState = TrayState.Empty;
    }
}
