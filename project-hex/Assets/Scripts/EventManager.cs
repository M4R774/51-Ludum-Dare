using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void VisibilityChange();
    public static event VisibilityChange OnVisibilityChange;

    public delegate void EndTurn();
    public static event EndTurn OnEndTurn;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void VisibilityHasChanged()
    {
        OnVisibilityChange?.Invoke();
    }
    public static void TurnHasEnded()
    {
        OnEndTurn?.Invoke();
    }

    public void Play()
    {
        audioSource.Play();
    }
}
