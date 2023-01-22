using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DonutStation : MonoBehaviour
{
    public enum State { Empty, Start, Half, Finished, Cooldown, Broken};

    [SerializeField] private int updateTime;
    [SerializeField] private int cooldownTime;

    // State
    [SerializeField] private GameObject sphere;
    [SerializeField] private GameObject sphereTR;
    [HideInInspector] private MeshRenderer sphereMaterial;
    [SerializeField] private Material red;
    [SerializeField] private Material orange;
    [SerializeField] private Material yellow;
    [SerializeField] private Material green;
    [SerializeField] private Material grey;
    [SerializeField] private Material purple;

    [SerializeField] private MeshRenderer machine;
    private Material initialMachineMaterial;

    // Input
    [SerializeField] private InputActionReference playerGrab;

    [SerializeField] private GameObject start;
    [SerializeField] private GameObject half;
    [SerializeField] private GameObject finished;

    private GameObject player;
    private State currentState = State.Empty;
    public State CurrentState => currentState;
    [HideInInspector] public State newState = State.Empty;
    private bool isBaking;
    private float timer;
    private int counter;
    private Vector3 initScale;

    [SerializeField] private PlayerState player1;
    [SerializeField] private PlayerState player2;

    private void Awake()
    {
        currentState = State.Empty;
        initialMachineMaterial = machine.material;
        sphereMaterial = sphere.GetComponent<MeshRenderer>();
        sphereMaterial.material = red;
        initScale = sphere.transform.localScale;
        start.SetActive(false);
        half.SetActive(false);
        finished.SetActive(false);
        sphereTR.SetActive(false);
        playerGrab.action.Enable();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = collider.gameObject;
            playerGrab.action.performed += GrabDonut;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player2" && collider.gameObject.GetComponent<PlayerState>().hasInteracted && !collider.gameObject.GetComponent<PlayerState>().interactionDone)
        {
            player = collider.gameObject;
            DonutInteraction();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Player2")
        {
            player = collider.gameObject;
            playerGrab.action.performed -= GrabDonut;
            player.GetComponent<PlayerState>().hasInteracted = false;
            if (currentState != State.Finished)
                Restart();
        }
    }

    private void GrabDonut(InputAction.CallbackContext context)
    {
        player.GetComponent<PlayerState>().hasInteracted = true;
        DonutInteraction();
    }

    private void DonutInteraction()
    {
        if (currentState == State.Broken)
            FixMachine();

        else if (currentState == State.Empty)
        {
            isBaking = true;
            currentState = State.Start;
            start.SetActive(true);
            half.SetActive(false);
            finished.SetActive(false);
            sphereMaterial.material = yellow;
            sphereTR.SetActive(true);
            timer = 0.0f;
        }
        else if (currentState == State.Finished && player.GetComponent<PlayerState>().currentState == PlayerState.State.None)
        {
            player.GetComponent<PlayerState>().currentState = PlayerState.State.Donut;
            Restart();
        }
    }

    private void FixMachine()
    {
        machine.material = initialMachineMaterial;
        StartCoroutine(Cooldown());
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

    private void Update()
    {
        if (currentState != newState || currentState == State.Broken)
        {
            counter++;
            if (counter > 300)
            {
                player1.currentState = PlayerState.State.None;
                player2.currentState = PlayerState.State.None;

                counter = 0;
                StopAllCoroutines();
                Restart();

                currentState = State.Broken;

                sphereMaterial.material = purple;
                machine.material = grey;
            }
        }


        if (!isBaking)
            return;

        timer += Time.deltaTime;
        if(currentState != State.Finished)
            sphere.transform.localScale += new Vector3(0.000009f, 0.000025f, 0.000009f);

        if (timer > updateTime)
        {
            switch(currentState)
            {
                case State.Start:
                    currentState = State.Half;
                    sphereMaterial.material = orange;
                    start.SetActive(false);
                    finished.SetActive(false);
                    half.SetActive(true);
                    break;
                case State.Half:
                    currentState = State.Finished;
                    sphereMaterial.material = green;
                    half.SetActive(false);
                    start.SetActive(false);
                    finished.SetActive(true);
                    sphereTR.SetActive(false);
                    player.GetComponent<PlayerState>().hasInteracted = false;
                    break;
            }
            timer = 0.0f;
        }
    }

    private void Restart()
    {
        timer = 0.0f;
        isBaking = false;
        currentState = State.Empty;
        start.SetActive(false);
        half.SetActive(false);
        finished.SetActive(false);
        sphereTR.SetActive(false);
        sphereMaterial.material = red;
        sphere.transform.localScale = initScale;
    }

    private void OnDisable()
    {
        playerGrab.action.Disable();
    }
}
