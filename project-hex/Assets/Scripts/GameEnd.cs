using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    public GameObject enemyTank;
    public float endingDelayInSeconds;
    public String victoryScreenName;
    public String losingScreenName;

    private bool gameHasAlreadyEnded;

    private void Awake()
    {
        gameHasAlreadyEnded = false;
    }

    private void Update()
    {
        CheckIfGameHasEnded();

        // debugging purposes
        if(Input.GetKeyDown(KeyCode.J)) PlayerWon();
    }

    private void CheckIfGameHasEnded()
    {
        if (!gameHasAlreadyEnded)
        {
            if (TurnManager.instance.playerControlledUnits.Count == 0)
            {
                PlayerLost();
            }
        }
    }

    private bool AnyPlayerControlledUnitsAreInGoal()
    {
        foreach (GameObject unit in TurnManager.instance.playerControlledUnits)
        {
            if (unit.GetComponent<ISelectable>().GetTileCoordinates() == GoalManager.instance.GetTileCoordinates())
            {
                return true;
            }
        }
        return false;
    }

    private void PlayerWon()
    {
        StartCoroutine(LoadEndingScreenAfterDelay(endingDelayInSeconds, victoryScreenName));
    }

    private void PlayerLost()
    {
        StartCoroutine(LoadEndingScreenAfterDelay(endingDelayInSeconds, losingScreenName));
    }

    private IEnumerator LoadEndingScreenAfterDelay(float delayInSeconds, String endingScreenName)
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(endingScreenName);
    }

    // can be used for triggering either scenarion
    public void TriggerGameEnd(bool condition)
    {
        if(condition) PlayerWon();
        else PlayerLost();
    }
}
