using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class Timer : MonoBehaviour
{
    public float timerInSeconds;

    private Slider slider;
    private float timeLeft;

    public void Start()
    {
        timeLeft = timerInSeconds;
        slider = GetComponent<Slider>();
    }

    public void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = timerInSeconds;
            if (timeLeft == 10)
            {
                EventManager.TenSecondTimerHasEnded();
            }
        }
        slider.value = timeLeft / timerInSeconds;
    }
}
