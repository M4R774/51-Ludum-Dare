using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;
    public int goalDistanceFromStartingPosition;
    public GameObject boss;
    [SerializeField] Animator animator;

    private AudioSource audioSource;

    public void Awake()
    {
        CheckThatIamOnlyInstance();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnGoalAndBossAwayFromStart(goalDistanceFromStartingPosition);
        MoveEnemyToGoal();
    }

    private void MoveEnemyToGoal()
    {
        boss.transform.position = transform.position;
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


    private void SpawnGoalAndBossAwayFromStart(int radius)
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
            audioSource.Play();
            animator.SetTrigger("openLid");
            GameObject.Find("GameControllers").GetComponent<GameEnd>().TriggerGameEnd(true);
        }
    }
}
