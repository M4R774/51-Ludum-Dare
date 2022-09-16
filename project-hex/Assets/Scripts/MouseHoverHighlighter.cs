using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverHighlighter : MonoBehaviour
{
    public GridLayout gridLayout;
    public float lightHeight;

    private RaycastHit hit;
    private Ray ray;
    private Vector3 mousePosition;
    private Vector3Int tilePosUnderMouse;
    private List<IHighlightable> highLightedObjects;

    void Start()
    {
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
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mousePosition = hit.point;
            mousePosition.y = 0;
            tilePosUnderMouse = gridLayout.WorldToCell(mousePosition);
            transform.position = gridLayout.CellToWorld(tilePosUnderMouse) + new Vector3(0, lightHeight, 0);

            IHighlightable objectToHighlight = hit.transform.GetComponent<IHighlightable>();
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

            DrawDebugLines();
        }
    }

    void DrawDebugLines()
    {
        // Debug
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), ray.direction * 20, Color.red);
        Debug.DrawLine(hit.point, hit.point + Vector3.left, Color.blue);
        Debug.DrawLine(hit.point, hit.point + Vector3.back, Color.green);
        Debug.DrawLine(hit.point, hit.point + Vector3.up, Color.cyan);
    }
}
