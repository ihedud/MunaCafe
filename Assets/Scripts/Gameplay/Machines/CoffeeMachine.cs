using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoffeeMachine : MonoBehaviour
{
    public enum State { Empty, Brewing, Done, Cooldown, Broken};

    [SerializeField] private int brewingTime;
    [SerializeField] private int cooldownTime;

    // State
    [SerializeField] private GameObject sphere;
    [HideInInspector] private MeshRenderer sphereMaterial;
    [SerializeField] private Material red;
    [SerializeField] private Material orange;
    [SerializeField] private Material green;
    [SerializeField] private Material grey;
    [SerializeField] private Material purple;

    [SerializeField] private MeshRenderer machine;
    private Material initialMachineMaterial;

    // Input
    [SerializeField] private InputActionReference playerGrab;

    [SerializeField] private GameObject coffee;
    [SerializeField] private GameObject mug;

    private GameObject player;
    private State currentState = State.Empty;
    public State CurrentState => currentState;
    [HideInInspector] public State newState = State.Empty;
    private int counter = 0;

    private void Awake()
    {
        initialMachineMaterial = machine.material;
        sphereMaterial = sphere.GetComponent<MeshRenderer>();
        sphereMaterial.material = red;
        currentState = State.Empty;
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
        if (currentState == State.Broken)
            FixMachine();
        else if (currentState == State.Empty)
            StartCoroutine(Brewing());

        if (currentState == State.Done && player.GetComponent<PlayerState>().currentState == PlayerState.State.None)
        {
            coffee.SetActive(false);
            mug.SetActive(false);

            player.GetComponent<PlayerState>().currentState = PlayerState.State.Coffee;

            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        currentState = State.Cooldown;
        sphereMaterial.material = grey;

        yield return new WaitForSeconds(cooldownTime);

        currentState = State.Empty;
        sphereMaterial.material = red;

        player.GetComponent<PlayerState>().hasInteracted = false;
    }

    private IEnumerator Brewing()
    {
        mug.SetActive(true);
        currentState = State.Brewing;
        sphereMaterial.material = orange;

        yield return new WaitForSeconds(brewingTime);

        currentState = State.Done;
        sphereMaterial.material = green;

        coffee.SetActive(true);

        player.GetComponent<PlayerState>().hasInteracted = false;
    }

    private void FixMachine()
    {
        machine.material = initialMachineMaterial;
        StartCoroutine(Cooldown());
    }

    private void Update()
    {
        if (currentState == newState && currentState != State.Broken)
            return;

        counter++;
        if (counter > 200 || newState == State.Broken)
        {
            Debug.Log("It broke lol");
            Debug.Log("My state: " + currentState + ", new state = " + newState);

            counter = 0;
            StopAllCoroutines();
            currentState = State.Broken;

            sphereMaterial.material = purple;
            machine.material = grey;

            coffee.SetActive(false);
            mug.SetActive(false);
        }
    }
}
