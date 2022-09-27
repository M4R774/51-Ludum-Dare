using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<GameObject> playerControlledUnits;
    public MouseController mouseController;
    public CameraController cameraController;
    public bool automaticallyEndTurn;

    private int indexOfLastSelectedUnit;

    public void SelectNextUnit()
    {
        GameObject selectedUnit = AnyUnitIsSelected();

        for (int i = 0; i < playerControlledUnits.Count; i++)
        {
            int indexOfNewUnit = (indexOfLastSelectedUnit + i) % playerControlledUnits.Count;
            if (selectedUnit != playerControlledUnits[indexOfNewUnit] && 
                playerControlledUnits[indexOfNewUnit].GetComponent<ISelectable>().MovementPointsLeft() > 0)
            {
                indexOfLastSelectedUnit = indexOfNewUnit;
                mouseController.SetSelectedObject(playerControlledUnits[indexOfNewUnit].GetComponent<ISelectable>());
                break;
            }
        }
    }

    private void CheckIfTurnCanBeAutomaticallyEndedOrGoToNextUnit()
    {
        GameObject selectedUnit = AnyUnitIsSelected();
        GameObject unitWithActionsLeft = GetAnyUnitThatHasActionsLeft();

        if (selectedUnit.GetComponent<ISelectable>().MovementPointsLeft() <= 0 && 
            unitWithActionsLeft != null)
        {
            mouseController.SetSelectedObject(unitWithActionsLeft.GetComponent<ISelectable>());
        }
        else if (automaticallyEndTurn)
        {
            EventManager.TurnHasEnded();
        }
    }

    private void SelectNewUnitOnEndTurn()
    {
        GameObject unitWithActionsLeft = AnyUnitIsSelected();
        if (unitWithActionsLeft == null)
        {
            unitWithActionsLeft = playerControlledUnits[0];
            mouseController.SetSelectedObject(unitWithActionsLeft.GetComponent<ISelectable>());
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
        EventManager.OnEndTurn += SelectNewUnitOnEndTurn;
    }

    private void OnDisable()
    {
        EventManager.OnMaybeEndTurn -= CheckIfTurnCanBeAutomaticallyEndedOrGoToNextUnit;
        EventManager.OnEndTurn -= SelectNewUnitOnEndTurn;
    }
}
