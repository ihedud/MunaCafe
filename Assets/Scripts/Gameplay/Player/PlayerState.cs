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
            coffee.SetActive(true);
        else if (currentState == State.EmptyTea && !emptyTea.activeSelf)
            emptyTea.SetActive(true);
        else if (currentState == State.Tea && !tea.activeSelf)
            tea.SetActive(true);
        else if (currentState == State.BurnedTea && !burnedTea.activeSelf)
            burnedTea.SetActive(true);
        else if (currentState == State.Donut && !donut.activeSelf)
            donut.SetActive(true);
        else if (currentState == State.None)
        {
            coffee.SetActive(false);
            emptyTea.SetActive(false);
            tea.SetActive(false);
            burnedTea.SetActive(false);
            donut.SetActive(false);
        }
    }

    public void SetDonutMesh(Mesh mesh)
    {
        donut.GetComponent<MeshFilter>().mesh = mesh;
    }
    public Mesh GetDonutMesh()
    {
        return donut.GetComponent<MeshFilter>().mesh;
    }
}
