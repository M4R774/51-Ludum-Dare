using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpawnGoalAwayFromPlayer(4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnGoalAwayFromPlayer(int radius)
    {
        Vector3Int initialPlayerCellCoordinates = new Vector3Int(0, 0, 0);
        List<Vector3Int> outerEdge = Pathfinding.GetRingOfRadius(initialPlayerCellCoordinates, radius);
        int randEdgePoint = Random.Range(0, outerEdge.Count);
        Vector3 spawnPosition = Pathfinding.gridLayout.CellToWorld(outerEdge[randEdgePoint]);
        transform.position = spawnPosition;
    }
}
