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
    private AudioSource audioSource;

    public void Start()
    {
        timeLeft = timerInSeconds;
        slider = GetComponent<Slider>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft < 0)
        {
            audioSource.Play();
            timeLeft = timerInSeconds;
            if (timeLeft == 10)
            {
                EventManager.TenSecondTimerHasEnded();
            }
            else
            {
                EventManager.ShortTimerHasEnded();
            }
        }
        slider.value = timeLeft / timerInSeconds;
    }
}
