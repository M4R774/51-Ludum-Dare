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


    // Variables needed for pathfinding
    public bool Walkable { get; set; }
    public int DistanceFromStart { get; set; }
    public int DistanceToTarget { get; set; }
    public WorldTile NextDestination { get; set; }

    private bool isVisible;
    private bool isExplored;
    private bool isWithinMovementRange;

    public bool IsVisible 
    {
        get { return isVisible; }
        set
        {
            isVisible = value;
            if (isVisible)
            {
                isExplored = true;
            }
            CalculateAndSetTileColor();
        }
    }

    public bool IsExplored 
    {
        get { return isExplored; }
        set 
        {
            isExplored = value;
            CalculateAndSetTileColor();
        }
    }

    public bool IsWithinMovementRange
    {
        get { return isWithinMovementRange; }
        set
        {
            isWithinMovementRange = value;
            CalculateAndSetTileColor();
        }
    }

    public int GetTotalPathCost()
    {
        return DistanceFromStart + DistanceToTarget;
    }

    public List<WorldTile> Neighbors()
    {
        List<List<Vector3Int>> precalculatedDirections = GetPrecalculatedNeighbourDirections();
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

        return neighbors;
    }

    private void CalculateAndSetTileColor()
    {
        if (isVisible && !isWithinMovementRange)
        {
            SetColor(Color.white);
        }
        else if (isWithinMovementRange && isVisible)
        {
            Color lightGreen = new(.7f, 1, .7f, 1);
            SetColor(lightGreen);
        }
        else if (isWithinMovementRange && !isVisible && isExplored)
        {
            Color darkGreen = new(.35f, .5f, .35f, 1);
            SetColor(darkGreen);
        }
        else if (isWithinMovementRange && !isVisible && !isExplored)
        {
            Color darkGreen = new(.35f, .5f, .35f, .5f);
            SetColor(darkGreen);
        }
        else if (isExplored)
        {
            SetColor(Color.grey);
        }
        else 
        {
            Color invisible = new(0,0,0,0);
            SetColor(invisible);
        }
    }

    private void SetColor(Color color)
    {
        if (TilemapMember != null)
        {
            TilemapMember.SetTileFlags(CellCoordinates, TileFlags.None);
            TilemapMember.SetColor(CellCoordinates, color);
        }
    }

    private List<List<Vector3Int>> GetPrecalculatedNeighbourDirections()
    {
        List<List<Vector3Int>> precalculatedDirections = new();
        precalculatedDirections.Add(GetNeighbourDirectionsForEvenHex());
        precalculatedDirections.Add(GetNeighbourDirectionsForOddHex());
        return precalculatedDirections;
    }

    private List<Vector3Int> GetNeighbourDirectionsForEvenHex()
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

    private List<Vector3Int> GetNeighbourDirectionsForOddHex()
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
