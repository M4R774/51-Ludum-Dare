using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseController : MonoBehaviour
{
    public GridLayout gridLayout;

    // Pathfinding tests
    private ISelectable selectedObject;

    private void Start()
    {
        selectedObject = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0)
        {
            GetWorldlTileUnderMouse(out WorldTile clickedTile, out ISelectable clickedSelectable);
            HandleSelectable(clickedSelectable);
            HandleTile(clickedTile, clickedSelectable);
        }
    }

    private void HandleSelectable(ISelectable clickedSelectable)
    {
        if (clickedSelectable != null)
        {
            if (clickedSelectable.IsSelected())
            {
                clickedSelectable.Unselect();
                selectedObject = null;
            }
            else
            {
                selectedObject?.Unselect();
                clickedSelectable.Select();
                selectedObject = clickedSelectable;
            }
        }
    }

    private void HandleTile(WorldTile clickedTile, ISelectable clickeckSelectable)
    {
        if (MovementIsPossible(clickedTile, clickeckSelectable))
        {
            Vector3Int selectedObjectCoordinates = selectedObject.GetTileCoordinates();
            GameTiles.instance.tiles.TryGetValue(selectedObjectCoordinates, out WorldTile tile);
            List<WorldTile> path = Pathfinding.FindPath(tile, clickedTile);
            selectedObject.MoveTowardsTarget(path);
        }
    }

    private void GetWorldlTileUnderMouse(out WorldTile _tile, out ISelectable _selectable)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        _selectable = null;
        _tile = null;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _selectable = hit.transform.GetComponent<ISelectable>();
            Vector3 mousePosition = hit.point;
            Vector3Int tileCoordinatesUnderMouse = gridLayout.WorldToCell(mousePosition);
            var tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles
            tiles.TryGetValue(tileCoordinatesUnderMouse, out _tile);
        }
    }

    private bool MovementIsPossible(WorldTile clickedTile, ISelectable clickedSelectable)
    {
        if (clickedTile != null &&
            clickedSelectable == null &&
            selectedObject != null &&
            clickedTile.CanEndTurnHere() &&
            clickedTile.IsVisible)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
