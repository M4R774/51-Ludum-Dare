using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;
    public int maxEnemyToGoal;
    public int goalDistanceFromStartingPosition;

    public void Awake()
    {
        CheckThatIamOnlyInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnGoalAwayFromPlayer(goalDistanceFromStartingPosition);
        MoveEnemyToMidway();
    }

    private void MoveEnemyToMidway()
    {
        List<WorldTile> tilesBetweenPlayerAndGoalPath = Pathfinding.FindPath(
            TurnManager.instance.playerControlledUnits[0].GetComponent<ISelectable>().GetTileUnderMyself(),
            GetTileUnderMyself()
        );
        int enemyToGoal = Mathf.Min(tilesBetweenPlayerAndGoalPath.Count - 1, maxEnemyToGoal);
        EnemyAI.instance.transform.position = tilesBetweenPlayerAndGoalPath[enemyToGoal].WorldPosition;
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
        Vector3Int initialPlayerCellCoordinates = new(0, 0, 0);
        List<Vector3Int> outerEdge = Pathfinding.GetRingOfRadius(initialPlayerCellCoordinates, radius);

        for (int i = 0; i < outerEdge.Count; i++)
        {
            Vector3 spawnPositionCandidate = Pathfinding.gridLayout.CellToWorld(outerEdge[Random.Range(0, outerEdge.Count)]);
            WorldTile tileUnderPossibleSpawnPosition = GameTiles.instance.GetTileByWorldPosition(spawnPositionCandidate);
            if (tileUnderPossibleSpawnPosition.IsWalkable()) 
            {
                transform.position = spawnPositionCandidate;
                return;
            }
        }
    }

    public Vector3Int GetTileCoordinates()
    {
        Vector3 tilePosition = transform.position;
        tilePosition.y = 0;
        return Pathfinding.gridLayout.WorldToCell(tilePosition);
    }

    public WorldTile GetTileUnderMyself()
    {
        WorldTile tileUnderMe = GameTiles.instance.GetTileByWorldPosition(transform.position);
        return tileUnderMe;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameObject.Find("GameControllers").GetComponent<GameEnd>().TriggerGameEnd(true);
        }
    }
}
