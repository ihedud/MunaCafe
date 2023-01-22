using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum State { None, Coffee, EmptyTea, Tea, BurnedTea, Donut };

    [HideInInspector] public State currentState;

    [HideInInspector] public bool hasInteracted;
    [HideInInspector] public bool interactionDone;

    [SerializeField] private GameObject coffee;
    [SerializeField] private GameObject emptyTea;
    [SerializeField] private GameObject tea;
    [SerializeField] private GameObject burnedTea;
    [SerializeField] private GameObject donut;

    private void Awake()
    {
        currentState = State.None;
        hasInteracted = false;
        interactionDone = false;
    }

    private void Update()
    {
        if (currentState == State.Coffee && !coffee.activeSelf)
        {
            DectivateProps();
            coffee.SetActive(true);
        }
        else if (currentState == State.EmptyTea && !emptyTea.activeSelf)
        {
            DectivateProps();
            emptyTea.SetActive(true);
        }
        else if (currentState == State.Tea && !tea.activeSelf)
        {
            DectivateProps();
            tea.SetActive(true);
        }
        else if (currentState == State.BurnedTea && !burnedTea.activeSelf)
        {
            DectivateProps();
            burnedTea.SetActive(true);
        }
        else if (currentState == State.Donut && !donut.activeSelf)
        {
            DectivateProps();
            donut.SetActive(true);
        }
        else if (currentState == State.None)
            DectivateProps();
    }

    private void DectivateProps()
    {
        coffee.SetActive(false);
        emptyTea.SetActive(false);
        tea.SetActive(false);
        burnedTea.SetActive(false);
        donut.SetActive(false);
    }
}
