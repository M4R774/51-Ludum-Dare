using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindControl : MonoBehaviour
{
    public static WindControl instance;

    private int windDirection;
    private List<int> movementCostDirections;

    public void Awake()
    {
        CheckThatIamOnlyInstance();
        movementCostDirections = new();
        movementCostDirections.Add(1);
        movementCostDirections.Add(1);
        movementCostDirections.Add(10);
        movementCostDirections.Add(1);
        movementCostDirections.Add(1);
        movementCostDirections.Add(1);
    }

    public int GetCostToDirection(int directionIndex)
    {
        // (indexOfLastSelectedUnit + i) % playerControlledUnits.Count
        int cost = movementCostDirections[(directionIndex + windDirection) % 6];
        return cost;
    }

    public int GetWindRotation()
    {
        return windDirection * -60;
    }

    private void CheckThatIamOnlyInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeWindDirection()
    {
        if (Random.Range(0,100) > 50)
        {
            windDirection = Random.Range(0, 6);
        }
    }
}
