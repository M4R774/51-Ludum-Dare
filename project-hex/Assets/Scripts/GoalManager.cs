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
        int randEdgePoint = Random.Range(0, outerEdge.Count);
        Vector3 spawnPosition = Pathfinding.gridLayout.CellToWorld(outerEdge[randEdgePoint]);
        transform.position = spawnPosition;
    }

    public Vector3Int GetTileCoordinates()
    {
        Vector3 tilePosition = transform.position;
        tilePosition.y = 0;
        return Pathfinding.gridLayout.WorldToCell(tilePosition);
    }
}
