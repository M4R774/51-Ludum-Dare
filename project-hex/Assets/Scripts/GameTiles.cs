using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour
{
	public static GameTiles instance;
	public Tilemap tilemap;
	public GridLayout grid;

	public Dictionary<Vector3, WorldTile> tiles;
    private readonly System.Random random = new();

    private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		GetWorldTiles();
	}

    private void Start()
    {
		Pathfinding.gridLayout = grid;
    }

    // Use this for initialization
    private void GetWorldTiles()
	{
		tiles = new Dictionary<Vector3, WorldTile>();
		foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
		{
			var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

			if (!tilemap.HasTile(localPlace)) continue;
			var tile = new WorldTile
			{
				CellCoordinates = localPlace,
				WorldPosition = tilemap.CellToWorld(localPlace),
				TileBase = tilemap.GetTile(localPlace),
				TilemapMember = tilemap,
				Name = localPlace.y + "," + localPlace.x,
				// Cost = 1 // TODO: Change this with the proper cost from ruletile
			};

			tiles.Add(localPlace, tile);
		}
		MakeRandomTilesUnwalkable();
	}

	private void MakeRandomTilesUnwalkable()
	{
		foreach (WorldTile tile in tiles.Values)
		{
			tile.Walkable = RandomBoolean(random);
			if (!tile.Walkable)
            {
				tile.SetColor(Color.black);
            }
		}
	}

	private bool RandomBoolean(System.Random random)
	{
		return random.Next() > (Int32.MaxValue / 4);
	}
}
