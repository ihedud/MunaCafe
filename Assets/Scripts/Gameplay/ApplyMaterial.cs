using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterial : MonoBehaviour
{
    [SerializeField] private int colorID = 0;
    [SerializeField] private Material material;

    [SerializeField] private GameObject uiCapsule;
    [SerializeField] private Client client;
    [SerializeField] private Host host;

    public void ChangeColor()
    {
        uiCapsule.GetComponent<MeshRenderer>().material = material;

        if (client != null)
        {
            client.colorID = colorID;
        }
        else if (host != null)
        {

            host.colorID = colorID;
        }
    }
}