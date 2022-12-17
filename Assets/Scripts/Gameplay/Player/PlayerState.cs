using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum State { None, Coffee };

    public State currentState;

    [SerializeField] private GameObject coffee;

    private void Awake()
    {
        currentState = State.None;
    }

    private void Update()
    {
        if(currentState == State.Coffee && !coffee.activeInHierarchy)
            coffee.SetActive(true);
        else if(currentState == State.None && coffee.activeInHierarchy)
            coffee.SetActive(false);
    }
}
