using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoScroll : MonoBehaviour
{
    public List<GameObject> playerControlledUnits;
    public MouseController mouseController;
    public CameraController cameraController;
    private bool cameraMoving;

    private void Start()
    {
        cameraMoving = false;
    }

    private void CheckIfTurnCanBeAutomaticallyEndedOrGoToNextUnit()
    {
        GameObject selectedUnit = AnyUnitIsSelected();
        if (selectedUnit != null)
        {
            if (selectedUnit.GetComponent<ISelectable>().MovementPointsLeft() <= 0)
            {
                GameObject unitWithActionsLeft = GetAnyUnitThatHasActionsLeft();
                if (unitWithActionsLeft != null)
                {
                    mouseController.SetSelectedObject(unitWithActionsLeft.GetComponent<ISelectable>());
                    cameraController.SetNewPosition(unitWithActionsLeft.transform);
                }
                else
                {
                    EventManager.TurnHasEnded();
                }
            }
        }
        else
        {
            GameObject unitWithActionsLeft = GetAnyUnitThatHasActionsLeft();
            if (unitWithActionsLeft != null)
            {
                mouseController.SetSelectedObject(unitWithActionsLeft.GetComponent<ISelectable>());
                cameraController.SetNewPosition(unitWithActionsLeft.transform);
            }
            else
            {
                EventManager.TurnHasEnded();
            }
        }
    }

    private GameObject AnyUnitIsSelected()
    {
        foreach (GameObject unit in playerControlledUnits)
        {
            if (unit.GetComponent<ISelectable>().IsSelected())
            {
                return unit;
            }
        }
        return null;
    }

    private GameObject GetAnyUnitThatHasActionsLeft()
    {
        foreach (GameObject unit in playerControlledUnits)
        {
            if (unit.GetComponent<ISelectable>().MovementPointsLeft() > 0)
            {
                return unit;
            }
        }
        return null;
    }

    private void OnEnable()
    {
        EventManager.OnMaybeEndTurn += CheckIfTurnCanBeAutomaticallyEndedOrGoToNextUnit;
    }

    private void OnDisable()
    {
        EventManager.OnMaybeEndTurn -= CheckIfTurnCanBeAutomaticallyEndedOrGoToNextUnit;
    }
}
