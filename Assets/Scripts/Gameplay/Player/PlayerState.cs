using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum State { None, Coffee };

    public State currentState;

    public bool hasInteracted;
    public bool interactionDone;

    [SerializeField] private GameObject coffee;

    private void Awake()
    {
        currentState = State.None;
        hasInteracted = false;
        interactionDone = false;
    }

    private void Update()
    {
        if(currentState == State.Coffee && !coffee.activeSelf)
            coffee.SetActive(true);
        else if(currentState == State.None && coffee.activeSelf)
            coffee.SetActive(false);
    }
}
