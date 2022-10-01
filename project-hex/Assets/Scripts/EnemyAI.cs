using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AbstractObjectInWorldSpace
{
    public int movementSpeed;

    private TurnManager turnManager;  // For accessing player controlled units
    private bool movementInProgress;
    private WorldTile tileUnderMe;

    void Start()
    {
        turnManager = TurnManager.instance;
        tileUnderMe = GetTileUnderMyself();
    }

    private void AIMoveAndAttack()
    {
        // Move 1 tile towards playerUnits[0]
        List<WorldTile> pathToPlayerUnit = Pathfinding.FindPath(GetTileUnderMyself(), turnManager.playerControlledUnits[0].GetComponent<CubeController>().GetTileUnderMyself());
        MoveTowardsTarget(pathToPlayerUnit);

        // Launch drone/barrage if in range

        // Fire laser/machinegun if line of sight to player
    }

    public void MoveTowardsTarget(List<WorldTile> path)
    {
        if (movementInProgress) return;
        StartCoroutine(LerpThroughPath(path));
    }

    private IEnumerator LerpThroughPath(List<WorldTile> path)
    {
        tileUnderMe.GameObjectOnTheTile = null;
        WorldTile endTile = path[^movementSpeed];
        tileUnderMe = endTile;
        endTile.GameObjectOnTheTile = transform.gameObject;

        movementInProgress = true;
        WorldTile startingTile = GetTileUnderMyself();
        for (int i = 0; i < movementSpeed; i++)
        {
            yield return LerpToNextTile(path, i);
        }

        EventManager.VisibilityHasChanged();
        movementInProgress = false;
    }

    private IEnumerator LerpToNextTile(List<WorldTile> path, int i)
    {
        Vector3 targetPosition = path[path.Count - 1 - i].WorldPosition;
        float elapsedTime = 0;
        float transitionTimeBetweenTiles = .3f;

        Vector3 velocity = Vector3.zero;
        float smoothTime = 0.1F;

        while (elapsedTime < transitionTimeBetweenTiles)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition;
    }

    private void OnEnable()
    {
        EventManager.OnTenSecondTimerEnded += AIMoveAndAttack;
    }

    private void OnDisable()
    {
        EventManager.OnTenSecondTimerEnded -= AIMoveAndAttack;
    }
}
