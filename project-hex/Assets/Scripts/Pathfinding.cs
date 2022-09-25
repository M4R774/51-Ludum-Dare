using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Pathfinding
{
    public static GridLayout gridLayout;

    public static List<WorldTile> GetAllTilesWithingMovementRange(WorldTile startTile, int range)
    {
        List<WorldTile> tilesWithinRange = new() { startTile };
        List<WorldTile> tilesToSearch = new();

        if (range <= 0)
        {
            return tilesWithinRange;
        }

        foreach (WorldTile initialNeighbour in startTile.Neighbors())
        {
            if (initialNeighbour.IsWalkable())
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
                    if (newNeighbour.IsWalkable() && !newNeighbors.Contains(newNeighbour))
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

    public static List<WorldTile> GetAllVisibleTiles(WorldTile startingTile, int visionRange)
    {
        List<WorldTile> visibleTiles = new() { startingTile };
        List<Vector3Int> outerEdge = GetRingOfRadius(startingTile.CellCoordinates, visionRange);

        foreach (Vector3Int edgeCoordinates in outerEdge)
        {
            for (int i = 0; i < visionRange + 1; i++)
            {
                Vector3 sampleWorldPosition = Vector3.Lerp(startingTile.WorldPosition,
                                                      gridLayout.CellToWorld(edgeCoordinates),
                                                      1.0f / visionRange * i);
                Vector3Int sampleTileCoordinates = gridLayout.WorldToCell(sampleWorldPosition);
                GameTiles.instance.tiles.TryGetValue(sampleTileCoordinates, out WorldTile sampleTile);
                if (sampleTile == null)
                {
                    break;
                }
                if (!visibleTiles.Contains(sampleTile))
                {
                    visibleTiles.Add(sampleTile);
                }
                if (sampleTile.BlocksVision() && i != 0)
                {
                    break;
                }
            }
        }

        return visibleTiles;
    }

    // this code doesn't work for radius == 0
    public static List<Vector3Int> GetRingOfRadius(Vector3Int gridCoordinates, int radius)
    {
        List<Vector3Int> ring = new();
        for (int i = 0; i < radius; i++)
        {
            gridCoordinates = NeighborGridCoordinates(gridCoordinates)[4];
        }

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                ring.Add(gridCoordinates);
                gridCoordinates = NeighborGridCoordinates(gridCoordinates)[i];
            }
        }

        return ring;
    }

    public static List<Vector3Int> NeighborGridCoordinates(Vector3Int CellCoordinates)
    {
        int evenOrOddColumn = CellCoordinates.y & 1;
        List<Vector3Int> neighborDirections = GetPrecalculatedNeighbourDirections()[evenOrOddColumn];

        List<Vector3Int> neighbors = new();
        foreach (Vector3Int neigborDirection in neighborDirections)
        {
            neighbors.Add(CellCoordinates + neigborDirection);
        }

        return neighbors;
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
                tile => !processed.Contains(tile) && tile.IsWalkable() && (tile.GameObjectOnTheTile == null || tile.GameObjectOnTheTile.GetComponent<HideIfNotVisible>() == null)))
            {
                var inSearch = toSearch.Contains(neighbor);

                int costToNeighbor = current.DistanceFromStart + 1; // + current.GetDistance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.DistanceFromStart)
                {
                    neighbor.DistanceFromStart = costToNeighbor;
                    neighbor.NextDestination = current;

                    if (!inSearch)
                    {
                        neighbor.DistanceToTarget = GetDistanceInTiles(neighbor, targetNode);
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

    private static int GetDistanceInTiles(WorldTile startTile, WorldTile targetTile)
    {
        var ac = OffsetCoordinatesToAxial(startTile.CellCoordinates);
        var bc = OffsetCoordinatesToAxial(targetTile.CellCoordinates);
        return AxialDistance(ac, bc);
    }

    public static List<List<Vector3Int>> GetPrecalculatedNeighbourDirections()
    {
        List<List<Vector3Int>> precalculatedDirections = new();
        precalculatedDirections.Add(GetNeighbourDirectionsForEvenHex());
        precalculatedDirections.Add(GetNeighbourDirectionsForOddHex());
        return precalculatedDirections;
    }

    private static List<Vector3Int> GetNeighbourDirectionsForEvenHex()
    {
        List<Vector3Int> even = new();
        even.Add(new Vector3Int(0, 1, 0));
        even.Add(new Vector3Int(-1, 1, 0));
        even.Add(new Vector3Int(-1, 0, 0));
        even.Add(new Vector3Int(-1, -1, 0));
        even.Add(new Vector3Int(0, -1, 0));
        even.Add(new Vector3Int(1, 0, 0));
        return even;
    }

    private static List<Vector3Int> GetNeighbourDirectionsForOddHex()
    {
        List<Vector3Int> odd = new();
        odd.Add(new Vector3Int(1, 1, 0));
        odd.Add(new Vector3Int(0, 1, 0));
        odd.Add(new Vector3Int(-1, 0, 0));
        odd.Add(new Vector3Int(0, -1, 0));
        odd.Add(new Vector3Int(1, -1, 0));
        odd.Add(new Vector3Int(1, 0, 0));
        return odd;
    }
}
