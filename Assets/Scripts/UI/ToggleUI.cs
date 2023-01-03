using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    [SerializeField] private GameObject deactivate;
    [SerializeField] private GameObject activate;

    public void Toggle()
    {
        deactivate.SetActive(false);
        activate.SetActive(true);
    }
}