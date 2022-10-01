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

    private void CheckIfGameHasEnded()
    {
        if (!gameHasAlreadyEnded)
        {
            if (TurnManager.instance.playerControlledUnits.Count == 0)
            {
                PlayerLost();
            }
            else if (enemyTank == null)
            {
                PlayerWon();
            }
        }
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

    private void OnEnable()
    {
        EventManager.OnTenSecondTimerEnded += CheckIfGameHasEnded;
    }

    private void OnDisable()
    {
        EventManager.OnTenSecondTimerEnded -= CheckIfGameHasEnded;
    }
}