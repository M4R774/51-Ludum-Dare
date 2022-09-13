using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour
{
	public static GameTiles instance;
	public Tilemap Tilemap;

	public Dictionary<Vector3, WorldTile> tiles;
    private System.Random random = new System.Random();

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

	// Use this for initialization
	private void GetWorldTiles()
	{
		tiles = new Dictionary<Vector3, WorldTile>();
		foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)
		{
			var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

			if (!Tilemap.HasTile(localPlace)) continue;
			var tile = new WorldTile
			{
				CellCoordinates = localPlace,
				WorldPosition = Tilemap.CellToWorld(localPlace),
				TileBase = Tilemap.GetTile(localPlace),
				TilemapMember = Tilemap,
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
