using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeaPot : MonoBehaviour
{
    public enum State { Empty, Brewing, Done, Burned, Cooldown, Broken };

    [SerializeField] private int brewingTime;
    [SerializeField] private int burningTime;
    [SerializeField] private int cooldownTime;

    // State
    [SerializeField] private GameObject sphere;
    [HideInInspector] private MeshRenderer sphereMaterial;
    [SerializeField] private Material red;
    [SerializeField] private Material orange;
    [SerializeField] private Material green;
    [SerializeField] private Material grey;
    [SerializeField] private Material black;
    [SerializeField] private Material purple;

    [SerializeField] private MeshRenderer machine;
    private Material initialMachineMaterial;

    // Input
    [SerializeField] private InputActionReference playerGrab;

    [SerializeField] private GameObject cup;

    private GameObject player;
    private int counter = 0;
    private State currentState = State.Empty;
    public State CurrentState => currentState;
    [HideInInspector] public State newState = State.Empty;

    [SerializeField] private PlayerState player1;
    [SerializeField] private PlayerState player2;

    private void Awake()
    {
        initialMachineMaterial = machine.material;
        sphereMaterial = sphere.GetComponent<MeshRenderer>();
        currentState = State.Empty;
        sphereMaterial.material = red;
        cup.SetActive(false);
        playerGrab.action.Enable();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = collider.gameObject;
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
        if (currentState == State.Broken)
            FixMachine();
        else if (currentState == State.Empty)
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
        sphereMaterial.material = grey;

        yield return new WaitForSeconds(cooldownTime);

        currentState = State.Empty;
        sphereMaterial.material = red;

        player.GetComponent<PlayerState>().hasInteracted = false;
    }

    private IEnumerator Brewing()
    {
        cup.SetActive(true);
        currentState = State.Brewing;
        sphereMaterial.material = orange;

        yield return new WaitForSeconds(brewingTime);

        currentState = State.Done;
        sphereMaterial.material = green;

        yield return new WaitForSeconds(burningTime);

        if (currentState == State.Done)
        {
            currentState = State.Burned;
            sphereMaterial.material = black;
        }

        player.GetComponent<PlayerState>().hasInteracted = false;
    }
    private void FixMachine()
    {
        machine.material = initialMachineMaterial;
        StartCoroutine(Cooldown());
    }

    private void Update()
    {
        if (currentState == newState || currentState == State.Broken)
            return;

        counter++;
        if (counter > 300)
        {
            SetBroken();
        }
    }

    private void SetBroken()
    {
        player1.currentState = PlayerState.State.None;
        player2.currentState = PlayerState.State.None;

        counter = 0;
        StopAllCoroutines();
        currentState = State.Broken;

        sphereMaterial.material = purple;
        machine.material = grey;

        cup.SetActive(false);
    }

    private void OnDisable()
    {
        playerGrab.action.Disable();
    }
}