using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void VisibilityChange();
    public static event VisibilityChange OnVisibilityChange;

    public static void VisibilityHasChanged()
    {
        OnVisibilityChange?.Invoke();
    }
}
