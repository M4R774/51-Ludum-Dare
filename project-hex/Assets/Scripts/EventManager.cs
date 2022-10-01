using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void VisibilityChange();
    public static event VisibilityChange OnVisibilityChange;

    public delegate void EndTurn();
    public static event EndTurn OnEndTurn;

    public delegate void MaybeEndTurn();
    public static event MaybeEndTurn OnMaybeEndTurn;

    public delegate void TenSecondTimerEnded();
    public static event TenSecondTimerEnded OnTenSecondTimerEnded;

    public delegate void ShortTimerEnded();
    public static event ShortTimerEnded OnShortTimerEnded;


    public static void VisibilityHasChanged()
    {
        OnVisibilityChange?.Invoke();
    }

    public static void MaybeTurnHasEnded()
    {
        OnMaybeEndTurn?.Invoke();
    }

    public static void TurnHasEnded()
    {
        OnEndTurn?.Invoke();
    }

    public static void TenSecondTimerHasEnded()
    {
        OnTenSecondTimerEnded?.Invoke();
    }

    public static void ShortTimerHasEnded()
    {
        OnShortTimerEnded?.Invoke();
    }
}
