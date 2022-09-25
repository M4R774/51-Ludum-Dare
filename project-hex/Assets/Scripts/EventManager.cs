using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void VisibilityChange();
    public static event VisibilityChange OnVisibilityChange;

    public delegate void EndTurn();
    public static event EndTurn OnEndTurn;

    public static void VisibilityHasChanged()
    {
        OnVisibilityChange?.Invoke();
    }
    public static void TurnHasEnded()
    {
        OnEndTurn?.Invoke();
    }
}
