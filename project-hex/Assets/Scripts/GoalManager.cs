using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;

    public void Awake()
    {
        CheckThatIamOnlyInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnGoalAwayFromPlayer(15);
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


    private void SpawnGoalAwayFromPlayer(int radius)
    {
        Vector3Int initialPlayerCellCoordinates = new Vector3Int(0, 0, 0);
        List<Vector3Int> outerEdge = Pathfinding.GetRingOfRadius(initialPlayerCellCoordinates, radius);
        foreach (Vector3Int edgePoint in outerEdge)
        {
            Vector3 spawnPosition = Pathfinding.gridLayout.CellToWorld(edgePoint);
            WorldTile tileUnderPossibleSpawnPosition = GameTiles.instance.GetTileByWorldPosition(spawnPosition);
            if (tileUnderPossibleSpawnPosition.IsWalkable()) {
                transform.position = spawnPosition;
                return;
            }
        }
        transform.position = initialPlayerCellCoordinates;
    }

    public Vector3Int GetTileCoordinates()
    {
        Vector3 tilePosition = transform.position;
        tilePosition.y = 0;
        return Pathfinding.gridLayout.WorldToCell(tilePosition);
    }
}
