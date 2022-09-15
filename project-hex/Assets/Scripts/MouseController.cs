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
        if (Input.GetMouseButtonDown(0))
        {
            GetWorldlTileUnderMouse(out _tile, out _selectable);
            HandleSelectable(_selectable);
            HandleTile(_tile);
        }

        if (Input.GetMouseButtonDown(1))
        {
            GetWorldlTileUnderMouse(out _tile, out _selectable);
            if (_tile != null)
            {
                if (startTile != null)
                {
                    startTile.TilemapMember.SetColor(startTile.CellCoordinates, Color.white);
                }
                startTile = _tile;

                _tile.TilemapMember.SetTileFlags(startTile.CellCoordinates, TileFlags.None);
                _tile.TilemapMember.SetColor(startTile.CellCoordinates, Color.yellow);
                startTile = _tile;

                if (startTile != null && targetTile != null)
                    Pathfinding.FindPath(startTile, targetTile);
            }
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

    private void HandleTile(WorldTile tile)
    {
        if (_tile != null && _selectable == null)
        {
            targetTile = _tile;

            _tile.TilemapMember.SetTileFlags(_tile.CellCoordinates, TileFlags.None);
            _tile.TilemapMember.SetColor(_tile.CellCoordinates, Color.red);

            if (startTile != null && targetTile != null)
                Pathfinding.FindPath(startTile, targetTile);
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
