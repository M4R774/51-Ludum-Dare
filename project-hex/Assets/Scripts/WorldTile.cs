using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// A class for storing data of each hex. Every hex has a instance of this
// class and you can store data here. 
public class WorldTile
{
    public Vector3Int CellCoordinates { get; set; }
    public Vector3 WorldPosition { get; set; }
    public TileBase TileBase { get; set; }
    public Tilemap TilemapMember { get; set; }
    public string Name { get; set; }

    public void SetColor(Color color)
    {
        TilemapMember.SetTileFlags(CellCoordinates, TileFlags.None);
        TilemapMember.SetColor(CellCoordinates, color);
    }

    // Variables needed for pathfinding
    public bool Walkable { get; set; }
    public int DistanceFromStart { get; set; }
    public int DistanceToTarget { get; set; }
    public int GetTotalPathCost()
    {
        return DistanceFromStart + DistanceToTarget;
    }
    public WorldTile NextDestination { get; set; }
    public List<WorldTile> Neighbors()
    {
        List<Vector3Int> even = new();
        even.Add(new Vector3Int( 0, 1, 0 ));
        even.Add(new Vector3Int(-1, 1, 0 ));
        even.Add(new Vector3Int(-1, 0, 0 ));
        even.Add(new Vector3Int(-1,-1, 0 ));
        even.Add(new Vector3Int( 0,-1, 0 ));
        even.Add(new Vector3Int( 1, 0, 0 ));

        List<Vector3Int> odd = new();
        odd.Add(new Vector3Int( 1, 1, 0));
        odd.Add(new Vector3Int( 0, 1, 0));
        odd.Add(new Vector3Int(-1, 0, 0));
        odd.Add(new Vector3Int( 0,-1, 0));
        odd.Add(new Vector3Int( 1,-1, 0));
        odd.Add(new Vector3Int( 1, 0, 0));

        List<List<Vector3Int>> precalculatedDirections = new();
        precalculatedDirections.Add(even);
        precalculatedDirections.Add(odd);

        int evenOrOddColumn = CellCoordinates.y & 1;
        List<Vector3Int> neighborDirections = precalculatedDirections[evenOrOddColumn];

        List<WorldTile> neighbors = new();
        foreach (Vector3Int neigborDirection in neighborDirections)
        {
            var tiles = GameTiles.instance.tiles;
            if (tiles.TryGetValue(CellCoordinates + neigborDirection, out WorldTile _tile))
            {
                neighbors.Add(_tile);
            }
        }

        /*
        string debugString = "";
        foreach (WorldTile neighbor in neighbors)
        {
            debugString += neighbor.Name + "; ";
        }
        Debug.Log(debugString);
        */
        return neighbors;
    }

    public int GetDistance(WorldTile targetTile)
    {
        var ac = OffsetCoordinatesToAxial(CellCoordinates);
        var bc = OffsetCoordinatesToAxial(targetTile.CellCoordinates);
        return axialDistance(ac, bc);
    }

    public Vector3Int AxialToOffsetCoordinates(Vector3Int axialCoordinates)
    {
        int column = axialCoordinates.y;
        var row = axialCoordinates.x + (axialCoordinates.x - (axialCoordinates.x & 1)) / 2;
        return new Vector3Int(row, column, 0);
    }

    public Vector3Int OffsetCoordinatesToAxial(Vector3Int offsetCoordinates)
    {
        var x = offsetCoordinates.y;
        var y = offsetCoordinates.x - (offsetCoordinates.y - (offsetCoordinates.y & 1)) / 2;
        return new Vector3Int(x, y, 0);
    }

    public Vector3Int axialSubtract(Vector3Int tileA, Vector3Int tileB)
    {
        return new Vector3Int(
            tileA.y - tileB.y, 
            tileA.x - tileB.x, 
            0);
    }

    public int axialDistance(Vector3Int startTile, Vector3Int targetTile)
    {
        var vec = axialSubtract(startTile, targetTile);
        return ((Mathf.Abs(vec.y)
              + Mathf.Abs(vec.y + vec.x)
              + Mathf.Abs(vec.y)) / 2);
    }

}
