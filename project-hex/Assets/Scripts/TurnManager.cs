using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public List<GameObject> playerControlledUnits;
    public Button nextUnitButton;
    public MouseController mouseController;
    public CameraController cameraController;
    public bool automaticallyEndTurn;

    private int indexOfLastSelectedUnit;
    public static TurnManager instance;

    public void Awake()
    {
        CheckThatIamOnlyInstance();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && GetAnyUnitThatHasActionsLeft() != null)
        {
            SelectNextUnit();
            cameraController.SetNewPosition(mouseController.GetSelectedObject().GetTransform());
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EventManager.TurnHasEnded();
        }
        if (Input.GetKeyDown(KeyCode.Space) && mouseController.GetSelectedObject() != null)
        {
            mouseController.GetSelectedObject().SkipTurn();
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt) && mouseController.GetSelectedObject() != null)
        {
            cameraController.SetNewPosition(mouseController.GetSelectedObject().GetTransform());
        }
    }

    public void SelectNextUnit()
    {
        ISelectable selectedSelectable = mouseController.GetSelectedObject();
        GameObject selectedUnit = null;
        if (selectedSelectable != null)
        {
            selectedUnit = selectedSelectable.GetTransform().gameObject;
        }

        for (int i = 0; i < playerControlledUnits.Count; i++)
        {
            int indexOfNewUnit = (indexOfLastSelectedUnit + i) % playerControlledUnits.Count;
            if (selectedUnit != playerControlledUnits[indexOfNewUnit] &&
                IfGameObjectHasMovementPointsLeft(playerControlledUnits[indexOfNewUnit]))
            {
                indexOfLastSelectedUnit = indexOfNewUnit;
                mouseController.SetSelectedObject(playerControlledUnits[indexOfNewUnit].GetComponent<ISelectable>());
                break;
            }
        }
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

    private void CheckIfTurnCanBeAutomaticallyEndedOrGoToNextUnit()
    {
        GameObject selectedUnit = mouseController.GetSelectedObject().GetTransform().gameObject;
        GameObject unitWithActionsLeft = GetAnyUnitThatHasActionsLeft();
        if (!IfSelectableHasMovementPointsLeft(selectedUnit.GetComponent<ISelectable>()))
        {
            if (unitWithActionsLeft != null)
            {
                SelectNextUnit();
            }
            else if (unitWithActionsLeft == null)
            {
                nextUnitButton.interactable = false;
                mouseController.SetSelectedObject(null);
                if (automaticallyEndTurn)
                {
                    EventManager.TurnHasEnded();
                }
            }
        }
    }

    private void SelectNewUnitOnEndTurn()
    {
        nextUnitButton.interactable = true;

        if (mouseController.GetSelectedObject() == null)
        {
            GameObject unitWithActionsLeft = playerControlledUnits[0];
            mouseController.SetSelectedObject(unitWithActionsLeft.GetComponent<ISelectable>());
        }
    }

    private bool IfSelectableHasMovementPointsLeft(ISelectable selectedUnit)
    {
        if (selectedUnit.MovementPointsLeft() <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool IfGameObjectHasMovementPointsLeft(GameObject gameObject)
    {
        if (gameObject.GetComponent<ISelectable>().MovementPointsLeft() <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
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
