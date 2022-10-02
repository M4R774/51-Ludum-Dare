using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

// GameTiles is the World map, a collection of WorldTile objects
public class GameTiles : MonoBehaviour
{
	public GameObject rescuableCube;
	public List<Tile> tileTypes;
	public Tile middleTileOfIsland;
	public Tile outerTileOfIsland;
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

	internal int GetTileTypeCost(WorldTile tile)
	{
		return tileBaseToTileData[tile.TileBase].cost;
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
		//GenerateInactiveCubes();
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
		FillMapWithRandomTiles();
		AddIslandsToMap();
	}

	private void FillMapWithRandomTiles()
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
			GameObject instantiatedObject = tile.TilemapMember.GetInstantiatedObject(localPlace);
			if (instantiatedObject != null)
			{
				instantiatedObject.SetActive(false);
			}
		}
	}

	private void AddIslandsToMap()
    {
		foreach (WorldTile tile in tiles.Values)
		{
			if (UnityEngine.Random.Range(0, 200) < 1)
            {
				tilemap.SetTile(tile.CellCoordinates, middleTileOfIsland);
				tiles[tile.CellCoordinates].IsExplored = false;
			}
		}
	}

	// TODO: Change this to the bananas or something like that?
	private void GenerateInactiveCubes()
	{
		var rand = new System.Random();
		var items = new  List<int>();
		items.AddRange(Enumerable.Range(0, tiles.Count).OrderBy(i => rand.Next()).Take(6));

		foreach(var item in items)
		{
		    var worldPosition = tiles.ElementAt(item).Value.WorldPosition;
			GameObject createdCube = Instantiate(rescuableCube, worldPosition, rescuableCube.transform.rotation);
			TurnManager.instance.playerControlledUnits.Add(createdCube);
		}
	}
}
