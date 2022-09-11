using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHighlighter : MonoBehaviour
{
    public GridLayout gridLayout;

    private RaycastHit last_hit;
    private RaycastHit hit;
    private Ray ray;
    private Vector3 mousePosition;
    private Vector3Int tilePosUnderMouse;

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mousePosition = hit.point;
            mousePosition.y = hit.transform.position.y + 2;
            tilePosUnderMouse = gridLayout.WorldToCell(mousePosition);
            transform.position = gridLayout.CellToWorld(tilePosUnderMouse);
            transform.position = transform.position;

            IHighlightable object_to_highlight = hit.transform.GetComponent<IHighlightable>();
            if (object_to_highlight != null)
            {
                last_hit = hit;
                object_to_highlight.Highlight();
            }

            IHighlightable object_to_unlight = last_hit.transform.GetComponent<IHighlightable>();
            if (hit.transform != last_hit.transform && object_to_unlight != null)
            {
                object_to_unlight.Unlight();
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
