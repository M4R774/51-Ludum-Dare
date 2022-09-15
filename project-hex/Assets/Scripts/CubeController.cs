using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour, ISelectable, IHighlightable
{
    private bool isSelected;
    private int highlightLevel;
    private Material material;

    void Start()
    {
        isSelected = false;
        material = transform.GetComponent<Renderer>().material;
    }

    public void Select()
    {
        isSelected = true;
        DetermineEmissionAndColor();
    }

    public void Unselect()
    {
        isSelected = false;
        DetermineEmissionAndColor();
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetHighlightLevel(int levelOfHighlight)
    {
        highlightLevel = levelOfHighlight;
        DetermineEmissionAndColor();
    }

    private void DetermineEmissionAndColor()
    {
        if (highlightLevel >= 2 || isSelected)
        {
            material.SetColor("_EmissionColor", Color.yellow);
        }
        else if (highlightLevel == 1)
        {
            material.SetColor("_EmissionColor", Color.grey);
        }
        else
        {
            material.SetColor("_EmissionColor", Color.black);
        }
    }
}
