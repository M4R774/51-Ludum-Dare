using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
    public List<Image> actionPoints;
    List<Image> allChildren;
    int currentPlannedMoves = 0; // the amount of hexes the player is planning to move

    public int savedState;

    void Start()
    {
        actionPoints.Clear();
        int children = gameObject.transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            actionPoints.Add(gameObject.transform.GetChild(i).GetComponent<Image>());
            allChildren = actionPoints; //.Add(gameObject.transform.GetChild(i).GetComponent<Image>());
            savedState = actionPoints.Count;
        }

        for (int i = 0; i < actionPoints.Count; i++)
        {
            actionPoints[i].color = Color.gray;
        }

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)) AddActionPoint();
    }

    public void SetVisible(int movementPointsLeft)
    {
        for (int i = 0; i < actionPoints.Count; i++)
        {
            if(i < movementPointsLeft) {
                actionPoints[i].color = Color.white;
            } else {
                actionPoints[i].color = new Color (0, 0, 0, 0.1f);
            }
        }
        savedState = movementPointsLeft;
    }

    public void SetPlan(int plannedMoveCount)
    {
        // Plan next move BUT
        // first return from previous plan:
        SetVisible(savedState);

        // save plannedMoveCount so we can use it in updating the visuals
        currentPlannedMoves = plannedMoveCount;

        var greens = new List<int>();
        for (int i = 0; i < actionPoints.Count; i++) {
            if(
                actionPoints[i].color == Color.white
            ) {
                greens.Add(i);
            }
        }

        for (int i = 0; i < plannedMoveCount; i++)
        {
            int lastGreen = greens.Count - i - 1;
            if (lastGreen < greens.Count && greens.Count > 0)
            {
                actionPoints[greens[lastGreen]].color = Color.gray;
            }
        }
    }

    // Grants the player one action point more
    public void AddActionPoint()
    {
        /*if(actionPoints.Count < allChildren.Count)
        {
            Debug.Log("I received an action point :-)");
            allChildren[actionPoints.Count].enabled = true;
            actionPoints.Add(gameObject.transform.GetChild(actionPoints.Count).GetComponent<Image>());
            SetPlan(currentPlannedMoves);
        }*/
    }

    // Removes one action point from the player
    public void RemoveActionPoint()
    {
        /*if(actionPoints.Count > 1)
        {
            Debug.Log("I lost an action point :-(");
            allChildren[actionPoints.Count - 1].enabled = false;
            actionPoints.RemoveAt(actionPoints.Count - 1);
            SetPlan(currentPlannedMoves);
        }*/
    }

    private void OnEnable()
    {
        EventManager.OnTenSecondTimerEnded += RemoveActionPoint;
    }

    private void OnDisable()
    {
        EventManager.OnTenSecondTimerEnded -= RemoveActionPoint;
    }
}
