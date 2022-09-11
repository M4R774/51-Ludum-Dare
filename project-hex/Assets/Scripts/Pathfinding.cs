using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
	public GridLayout gridLayout;

	private WorldTile _tile;

    private static readonly Color PathColor = Color.red;
    private static readonly Color OpenColor = Color.green;
    private static readonly Color ClosedColor = Color.blue;

    // Pathfinding tests
    private WorldTile targetTile;
    private WorldTile startTile;

    private void Update()
	{
        if (Input.GetMouseButtonDown(0))
        {
            if (GetWorldlTileUnderMouse(out _tile))
            {
                Debug.Log("Tile " + _tile.Name);

                if (targetTile != null)
                {
                    targetTile.TilemapMember.SetColor(targetTile.CellCoordinates, Color.white);
                }
                targetTile = _tile;

                _tile.TilemapMember.SetTileFlags(_tile.CellCoordinates, TileFlags.None);
                _tile.TilemapMember.SetColor(_tile.CellCoordinates, Color.red);

                if (startTile != null && targetTile != null)
                    FindPath(startTile, targetTile);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (GetWorldlTileUnderMouse(out _tile))
            {
                if (startTile != null)
                {
                    startTile.TilemapMember.SetColor(startTile.CellCoordinates, Color.white);
                }
                startTile = _tile;

                _tile.TilemapMember.SetTileFlags(startTile.CellCoordinates, TileFlags.None);
                _tile.TilemapMember.SetColor(startTile.CellCoordinates, Color.yellow);
                startTile = _tile;

                if(startTile != null && targetTile != null)
                    FindPath(startTile, targetTile);
            }
        }
    }

    private bool GetWorldlTileUnderMouse(out WorldTile _tile)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        _tile = null;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 mousePosition = hit.point;
            Vector3Int tileCoordinatesUnderMouse = gridLayout.WorldToCell(mousePosition);
            var tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

            if (tiles.TryGetValue(tileCoordinatesUnderMouse, out _tile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    // This algorithm is written for readability. Although it would be perfectly fine in 80% of
    // games, please don't use this in an RTS without first applying some optimization mentioned
    // in the video:https://youtu.be/i0x5fj4PqP4
    //
    // Also, setting colors and text on each hex affects performance, so removing that will also 
    // improve it marginally.
    public static List<WorldTile> FindPath(WorldTile startNode, WorldTile targetNode)
    {
        var toSearch = new List<WorldTile>() { startNode };
        var processed = new List<WorldTile>();

        while (toSearch.Any())
        {
            var current = toSearch[0];
            foreach (WorldTile tile in toSearch)
            {
                if (tile.GetTotalPathCost() <= current.GetTotalPathCost() &&
                    tile.DistanceToTarget < current.DistanceToTarget)
                {
                    current = tile;
                }
            }
            processed.Add(current);
            toSearch.Remove(current);

            current.SetColor(ClosedColor);

            if (current == targetNode)
            {
                var currentPathTile = targetNode;
                var path = new List<WorldTile>();
                var count = 100;
                while (currentPathTile != startNode)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.NextDestination;
                    count--;
                    if (count < 0) throw new Exception();
                }

                WorldTile previousTile = null;
                foreach (var tile in path)
                {
                    if (previousTile != null)
                    {
                        Debug.DrawLine(previousTile.WorldPosition, tile.WorldPosition, Color.magenta, 3);
                    }
                    previousTile = tile;
                    tile.SetColor(PathColor);
                }
                startNode.SetColor(PathColor);
                Debug.Log(path.Count);
                return path;
            }

            foreach (var neighbor in current.Neighbors().Where(
                tile => !processed.Contains(tile)))  // && tile.Walkable
            {
                var inSearch = toSearch.Contains(neighbor);

                int costToNeighbor = current.DistanceFromStart + 1; // + current.GetDistance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.DistanceFromStart)
                {
                    neighbor.DistanceFromStart = costToNeighbor;
                    neighbor.NextDestination = current;

                    if (!inSearch)
                    {
                        neighbor.DistanceToTarget = neighbor.GetDistance(targetNode);
                        toSearch.Add(neighbor);
                        neighbor.SetColor(OpenColor);
                    }
                }
            }
        }
        return null;
    }
}
