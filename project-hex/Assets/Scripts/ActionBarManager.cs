using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
    public List<Image> indicatorImages;
    public GameObject player;

    private int currentPlannedMoves = 0; // the amount of hexes the player is planning to move
    private CubeController playerController;

    void Start()
    {
        CollectAllIndicatorImagesFromChildren();
        SetAllIndicatorImagesGray();
        playerController = player.GetComponent<CubeController>();
    }

    public void RefreshIndicatorColors()
    {
        for (int i = 0; i < indicatorImages.Count; i++)
        {
            indicatorImages[i].enabled = true;
            if (i < playerController.movementPointsLeft - currentPlannedMoves)
            {
                indicatorImages[i].color = Color.white;
            }
            else if (i < playerController.movementPointsLeft)
            {
                indicatorImages[i].color = Color.grey;
            }
            else if (i < playerController.movementSpeed)
            {
                indicatorImages[i].color = new Color(0, 0, 0, 0.2f);
            }
            else
            {
                indicatorImages[i].enabled = false;
            }
        }
    }

    public void SetPlan(int plannedMoveCount)
    {
        // Plan next move BUT
        // first return from previous plan:
        currentPlannedMoves = plannedMoveCount;
        RefreshIndicatorColors();
    }

    public void IncreaseMaxMovementPoints()
    {
        if(playerController.movementSpeed < indicatorImages.Count)
        {
            Debug.Log("I received an action point :-)");
            playerController.movementSpeed += 1;
            SetPlan(currentPlannedMoves);
        }
    }

    // Removes one action point from the player
    public void DecreaseMaxMovementPoints()
    {
        if(playerController.movementSpeed > 1)
        {
            playerController.movementSpeed -= 1;
            SetPlan(currentPlannedMoves);
        }
    }

    private void CollectAllIndicatorImagesFromChildren()
    {
        int childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            indicatorImages.Add(gameObject.transform.GetChild(i).GetComponent<Image>());
        }
    }

    private void SetAllIndicatorImagesGray()
    {
        for (int i = 0; i < indicatorImages.Count; i++)
        {
            indicatorImages[i].color = Color.gray;
        }
    }

    private void OnEnable()
    {
        EventManager.OnTenSecondTimerEnded += DecreaseMaxMovementPoints;
    }

    private void OnDisable()
    {
        EventManager.OnTenSecondTimerEnded -= DecreaseMaxMovementPoints;
    }
}
