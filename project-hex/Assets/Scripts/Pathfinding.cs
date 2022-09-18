using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Pathfinding
{
	public static GridLayout gridLayout;

    //private static readonly Color PathColor = Color.red;
    //private static readonly Color OpenColor = Color.green;
    //private static readonly Color ClosedColor = Color.blue;
    //static List<WorldTile> coloredTilesInPreviousRound = new();

    public static List<WorldTile> GetAllTilesWithingRange(WorldTile startTile, int range)
    {
        List<WorldTile> tilesWithinRange = new();
        
        for (int i = 0; i < range; i++)
        {
            //asdf
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
        //ClearAllColouredTiles();
        //coloredTilesInPreviousRound.Add(startNode);

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

            //current.SetColor(ClosedColor);

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
                    //tile.SetColor(PathColor);
                }
                //startNode.SetColor(PathColor);
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
                        neighbor.DistanceToTarget = neighbor.GetDistance(targetNode);
                        toSearch.Add(neighbor);
                        //neighbor.SetColor(OpenColor);
                        //coloredTilesInPreviousRound.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    /*
    private static void ClearAllColouredTiles()
    {
        foreach (WorldTile tile in coloredTilesInPreviousRound)
        {
            tile.SetColor(Color.white);
        }
        coloredTilesInPreviousRound.Clear();
    }
    */
}
