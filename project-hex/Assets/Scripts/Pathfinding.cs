using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Pathfinding
{
	public static GridLayout gridLayout;

    public static List<WorldTile> GetAllTilesWithingRange(WorldTile startTile, int range)
    {
        List<WorldTile> tilesWithinRange = new() { startTile };
        List<WorldTile> tilesToSearch = new();

        foreach (WorldTile initialNeighbour in startTile.Neighbors())
        {
            if (initialNeighbour.Walkable )
            {
                tilesWithinRange.Add(initialNeighbour);
                tilesToSearch.Add(initialNeighbour);
            }
        }

        for (int i = 1; i < range; i++)
        {
            List<WorldTile> newNeighbors = new();
            foreach (WorldTile tileToSearch in tilesToSearch)
            {
                foreach (WorldTile newNeighbour in tileToSearch.Neighbors())
                {
                    if (newNeighbour.Walkable && !newNeighbors.Contains(newNeighbour))
                    {
                        newNeighbors.Add(newNeighbour);
                    }
                }
            }
            tilesToSearch.Clear();
            newNeighbors = newNeighbors.Except(tilesWithinRange).ToList();
            tilesToSearch.AddRange(newNeighbors);
            tilesWithinRange.AddRange(newNeighbors);
        }

        return tilesWithinRange;
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
                        Debug.DrawLine(previousTile.WorldPosition, tile.WorldPosition, Color.magenta, 1);
                    }
                    previousTile = tile;
                }
                return path;
            }

            foreach (var neighbor in current.Neighbors().Where(
                tile => !processed.Contains(tile) && tile.Walkable))
            {
                var inSearch = toSearch.Contains(neighbor);

                int costToNeighbor = current.DistanceFromStart + 1; // + current.GetDistance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.DistanceFromStart)
                {
                    neighbor.DistanceFromStart = costToNeighbor;
                    neighbor.NextDestination = current;

                    if (!inSearch)
                    {
                        neighbor.DistanceToTarget = GetDistance(neighbor, targetNode);
                        toSearch.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    private static Vector3Int AxialToOffsetCoordinates(Vector3Int axialCoordinates)
    {
        int column = axialCoordinates.y;
        var row = axialCoordinates.x + (axialCoordinates.y - (axialCoordinates.y & 1)) / 2;
        return new Vector3Int(row, column, 0);
    }

    private static Vector3Int OffsetCoordinatesToAxial(Vector3Int offsetCoordinates)
    {
        var column = offsetCoordinates.y;
        var row = offsetCoordinates.x - (offsetCoordinates.y - (offsetCoordinates.y & 1)) / 2;
        return new Vector3Int(column, row, 0);
    }

    private static Vector3Int AxialSubtract(Vector3Int tileA, Vector3Int tileB)
    {
        return new Vector3Int(
            tileA.y - tileB.y,
            tileA.x - tileB.x,
            0);
    }

    private static int AxialDistance(Vector3Int startTile, Vector3Int targetTile)
    {
        var vec = AxialSubtract(startTile, targetTile);
        return ((Mathf.Abs(vec.y)
              + Mathf.Abs(vec.y + vec.x)
              + Mathf.Abs(vec.x)) / 2);
    }

    private static int GetDistance(WorldTile startTile, WorldTile targetTile)
    {
        var ac = OffsetCoordinatesToAxial(startTile.CellCoordinates);
        var bc = OffsetCoordinatesToAxial(targetTile.CellCoordinates);
        return AxialDistance(ac, bc);
    }
}
