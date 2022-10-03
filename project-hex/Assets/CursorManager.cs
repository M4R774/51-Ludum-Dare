using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTex;
    void Awake() { Cursor.SetCursor(cursorTex, new Vector2(6, 6), CursorMode.ForceSoftware); }
}