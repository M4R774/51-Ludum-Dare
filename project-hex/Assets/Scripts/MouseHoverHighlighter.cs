using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverHighlighter : MonoBehaviour
{
    public MouseController mouseController;
    public float lightHeight;

    private GridLayout gridLayout;
    private Ray ray;
    private Vector3 mousePosition;
    private Vector3Int tilePosUnderMouse;
    private List<IHighlightable> highLightedObjects;

    void Start()
    {
        gridLayout = mouseController.gridLayout;
        highLightedObjects = new();
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            HighlightObjectUnderMouse();
        }
    }

    private void HighlightObjectUnderMouse()
    {
        mousePosition = mouseController.hit.point;
        mousePosition.y = 0;
        tilePosUnderMouse = gridLayout.WorldToCell(mousePosition);
        transform.position = gridLayout.CellToWorld(tilePosUnderMouse) + new Vector3(0, lightHeight, 0);
        if (mouseController.hit.transform == null)
        {
            return;
        }
        IHighlightable objectToHighlight = mouseController.hit.transform.GetComponent<IHighlightable>();
        if (objectToHighlight != null)
        {
            highLightedObjects.Add(objectToHighlight);
            objectToHighlight.SetHighlightLevel(1);
        }

        foreach (IHighlightable objectToUnlight in highLightedObjects)
        {
            if (objectToUnlight != objectToHighlight)
            {
                objectToUnlight.SetHighlightLevel(0);
            }
        }
    }
}
