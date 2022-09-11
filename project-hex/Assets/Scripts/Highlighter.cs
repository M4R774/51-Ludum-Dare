using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour, IHighlightable
{
    private Material material;

    void Start()
    {
        material = transform.GetComponent<Renderer>().material;
    }

    public void Highlight()
    {
        material.SetColor("_EmissionColor", Color.grey);
    }

    public void Unlight()
    {
        material.SetColor("_EmissionColor", Color.black);
    }
}
