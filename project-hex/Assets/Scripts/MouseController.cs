using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseController : MonoBehaviour
{
    public GridLayout gridLayout;

    // Pathfinding tests
    private WorldTile targetTile;
    private WorldTile startTile;
    private WorldTile _tile;
    private ISelectable _selectable;
    private List<ISelectable> selectedObjects;

    private void Start()
    {
        selectedObjects = new();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0)
        {
            GetWorldlTileUnderMouse(out _tile, out _selectable);
            HandleSelectable(_selectable);
            HandleTile(_tile);
        }
    }

    private void HandleSelectable(ISelectable selectable)
    {
        if (selectable != null)
        {
            if (selectable.IsSelected())
            {
                selectable.Unselect();
                selectedObjects.Remove(_selectable);
            }
            else
            {
                selectable.Select();
                selectedObjects.Add(_selectable);
            }
        }
    }

    private void HandleTile(WorldTile clickedTile)
    {
        if (clickedTile != null && _selectable == null)
        {
            if (selectedObjects.Count > 0 && clickedTile != null)
            {
                Vector3Int selectedObjectCoordinates = selectedObjects[0].GetTileCoordinates();
                GameTiles.instance.tiles.TryGetValue(selectedObjectCoordinates, out _tile);
                if (clickedTile.Walkable)
                {
                    List<WorldTile> path = Pathfinding.FindPath(_tile, clickedTile);
                    MoveSelectedObjectTowardsTarget(path, 2);
                }
            }
        }
    }

    private void MoveSelectedObjectTowardsTarget(List<WorldTile> path, int movementSpeed)
    {
        int numberOfTilesToMove = movementSpeed;
        if (path.Count < movementSpeed)
        {
            numberOfTilesToMove = path.Count;
        }

        for (int i = 0; i< numberOfTilesToMove; i++)
        {
            selectedObjects[0].MoveToTile(path[path.Count-1-i].CellCoordinates);
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
}
