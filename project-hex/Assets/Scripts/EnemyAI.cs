using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AbstractObjectInWorldSpace
{
    public int movementSpeed;
    public int barrageRange;
    public GameObject projectilePrefab;
    public GameObject bulletPrefab;
    public bool fireMGInsteadOfBarrage;

    protected GridLayout grid;
    private GameObject player;
    private bool movementInProgress;
    private WorldTile tileUnderMe;

    public bool isAlive = true; // used to check that the enemy can move and shoot

    private Coroutine movementCoroutine;

    void Start()
    {
        grid = GameTiles.instance.grid;
        tileUnderMe = GetTileUnderMyself();
        player = GameObject.Find("Player");
    }

    private WorldTile GetTileUnderMyself()
    {
        if (transform != null)
        {
            WorldTile tileUnderCube = GameTiles.instance.GetTileByWorldPosition(transform.position);
            return tileUnderCube;
        }
        return null;
    }

    private void AIMoveAndAttack()
    {
        if(movementCoroutine == null)
        {
            movementCoroutine = StartCoroutine(AsyncAttack());
        }
        else
        {
            StopCoroutine(movementCoroutine); // making sure the coroutine isn't still running
            movementCoroutine = null;
            movementCoroutine = StartCoroutine(AsyncAttack());
        }
    }

    private void Attack()
    {
        if (TargetIsInRange())
        {
            if (fireMGInsteadOfBarrage)
            {
                FireMachineGun();
            }
            else
            {
                FireBarrage();
            }
        }
    }

    private void FireBarrage()
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = transform.position;
        projectile.GetComponent<ProjectileSlerp>().SlerpToTargetAndExplode(
            tileUnderMe.WorldPosition,
            player.transform.position
        );
    }

    private void FireMachineGun()
    {
        GameObject projectile = Instantiate(bulletPrefab);
        projectile.transform.position = transform.position;
        projectile.GetComponent<MGBulletLerp>().SlerpToTargetAndExplode(
            tileUnderMe.WorldPosition, 
            player.transform.position);
    }

    private void MoveTowardsTarget(List<WorldTile> path)
    {
        if (movementInProgress) return;
        if (path != null)
        {
            StartCoroutine(LerpThroughPath(path));
        }
    }

    private bool TargetIsInRange()
    {
        // first let's make sure if player is not already dead
        // this needs to be changed if we want enemies to destroy other ships than the player
        if(player != null)
        {
            ISelectable target = player.GetComponent<ISelectable>();
            int distanceToTarget = Pathfinding.GetDistanceInTiles(
                target.GetTileUnderMyself(), 
                GetTileUnderMyself());
            if (distanceToTarget < barrageRange)
            {
                return true;
            }
            return false;
        }
        else return false;
    }

    // Game stuttered every 10 seconds, wait for random time to fix
    private IEnumerator AsyncAttack()
    {
        if(isAlive) // Is the AI even alive?
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.0f, 7.0f));
            if (player != null)
            {
                List<WorldTile> pathToPlayerUnit = Pathfinding.FindPath(GetTileUnderMyself(), player.GetComponent<ISelectable>().GetTileUnderMyself());
                MoveTowardsTarget(pathToPlayerUnit);
            }
        }
        else // if not, stop the coroutine and leave
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
            yield return null;
        }  
    }

    private IEnumerator LerpThroughPath(List<WorldTile> path)
    {
        tileUnderMe.GameObjectOnTheTile = null;
        if (path.Count > movementSpeed)
        {
            WorldTile endTile = path[^movementSpeed];
            endTile.GameObjectOnTheTile = this.gameObject;
            tileUnderMe = endTile;

            movementInProgress = true;
            WorldTile startingTile = GetTileUnderMyself();
            for (int i = 0; i < movementSpeed; i++)
            {
                yield return LerpToNextTile(path, i);
            }
            Attack();
            EventManager.VisibilityHasChanged();
            movementInProgress = false;
        }
        movementCoroutine = null;
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

            // Rotation
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 4);

            yield return null;
        }

        transform.position = targetPosition;
    }

    private void OnDestroy()
    {
        //TODO: Spawn effects
        EventManager.MaybeGameHasEnded();
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
