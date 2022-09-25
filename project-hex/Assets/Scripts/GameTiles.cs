using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// GameTiles is the World map, a collection of WorldTile objects
public class GameTiles : MonoBehaviour
{
	public List<Tile> tileTypes;
	public List<TileData> tileDatas;
	public static GameTiles instance;
	public Tilemap tilemap;
	public GridLayout grid;
	public Dictionary<Vector3Int, WorldTile> tiles;

	private Dictionary<TileBase, TileData> tileBaseToTileData;

	public bool TileTypeIsWalkable(WorldTile tile)
    {
		return tileBaseToTileData[tile.TileBase].walkable;
	}

	public bool TileTypeBlocksVision(WorldTile tile)
	{
		return tileBaseToTileData[tile.TileBase].blocksVision;
	}

	public WorldTile GetTileByWorldPosition(Vector3 worldPosition)
    {
		worldPosition.y = 0;
		Vector3Int tileCoordinates = grid.WorldToCell(worldPosition);
		tiles.TryGetValue(tileCoordinates, out WorldTile tile);
		return tile;
	}

	private void Awake()
	{
		Pathfinding.gridLayout = grid;
		CheckThatIamOnlyInstance();
		GenerateDictTileBaseToTileData();
		GenerateMap();
	}

	private void CheckThatIamOnlyInstance()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	// Initialize the dictionary, so that the TileData class is connected with Tile types.
	private void GenerateDictTileBaseToTileData()
    {
		tileBaseToTileData = new();

		foreach (TileData tileData in tileDatas)
        {
			foreach (Tile tile in tileData.tiles)
            {
				tileBaseToTileData.Add(tile, tileData);
			}
		}
	}

	private void GenerateMap()
	{
		tiles = new Dictionary<Vector3Int, WorldTile>();
		foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
		{
			var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

			if (!tilemap.HasTile(localPlace)) continue;
			int randomTileIndex = UnityEngine.Random.Range(0, tileTypes.Count);
			Tile randomizedTileType = tileTypes[randomTileIndex];
			tilemap.SetTile(localPlace, randomizedTileType);

			WorldTile tile = new()
            {
				CellCoordinates = localPlace,
				WorldPosition = tilemap.CellToWorld(localPlace),
				TileBase = tilemap.GetTile(localPlace),
				TilemapMember = tilemap,
				Name = localPlace.y + "," + localPlace.x,
				IsVisible = false,
				IsExplored = false,
				IsWithinMovementRange = false,
			};
			tiles.Add(localPlace, tile);
		}
	}
}
