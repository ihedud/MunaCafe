﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterial : MonoBehaviour
{
    [SerializeField] private int colorID = 0;
    [SerializeField] private Material material;
    [SerializeField] private GameObject uiCapsule;

    public void ChangeColor()
    {
        uiCapsule.GetComponent<MeshRenderer>().material = material;
    }
}