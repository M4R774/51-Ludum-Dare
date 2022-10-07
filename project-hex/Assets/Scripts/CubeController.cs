using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeController : AbstractObjectInWorldSpace, ISelectable, IHighlightable
{
    public int visibilityRange;
    public int movementPointsLeft;
    public int movementSpeed;
    public bool isPlayable;

    public float smoothTime;
    public float transitionTimeBetweenTiles = .3f;

    [SerializeField] GameObject playerModel;

    private bool isSelected;
    private int highlightLevel;
    private Material material;
    private bool movementInProgress;
    private WorldTile tileUnderMe;

    public ActionBarManager actionBarManager;

    void Awake()
    {
        if(actionBarManager == null) actionBarManager = GameObject.Find("ActionBar").GetComponent<ActionBarManager>();
    }
    void Start()
    {
        movementInProgress = false;
        grid = GameTiles.instance.grid;
        isSelected = false;

        InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);

        tileUnderMe = GetTileUnderMyself();
        tileUnderMe.GameObjectOnTheTile = this.gameObject;

        visibilityRange = movementSpeed;
        movementPointsLeft = movementSpeed;

        if(playerModel == null) playerModel = this.gameObject;
    }

    public int MovementSpeed
    {
        get { return movementPointsLeft; }
        set { movementSpeed = value; }
    }

    public void Select()
    {
        if (isPlayable)
        {
            isSelected = true;
            actionBarManager.RefreshIndicatorColors();
            InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), movementPointsLeft, true);
        }

    }

    public void Unselect()
    {
        isSelected = false;
        InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), movementPointsLeft, false);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public bool IsPlayable()
    {
        return isPlayable;
    }

    public void SetHighlightLevel(int levelOfHighlight)
    {
        highlightLevel = levelOfHighlight;
    }

    public void MoveTowardsTarget(List<WorldTile> path)
    {
        if (movementInProgress) return;

        int numberOfTilesToMove = movementPointsLeft;
        if (path.Count < movementPointsLeft)
        {
            numberOfTilesToMove = path.Count;
        }

        if (numberOfTilesToMove <= 0)
        {
            return;
        }
        StartCoroutine(LerpThroughPath(path, numberOfTilesToMove));
    }

    public void ActivateSurroundingCubes()
    {
        tileUnderMe = GetTileUnderMyself();
        foreach (var neighbor in tileUnderMe.Neighbors().Where(
            tile => tile.GameObjectOnTheTile != null))
        {
            if (
                neighbor.GameObjectOnTheTile.GetComponent<CubeController>() != null &&
                !neighbor.GameObjectOnTheTile.GetComponent<CubeController>().IsPlayable()
            )
            {
                neighbor.GameObjectOnTheTile.GetComponent<CubeController>().isPlayable = true;
            }
        }
    }

    private void InformTilesIfTheyAreWithinVisionRange(WorldTile startTile, int range, bool isInRange)
    {
        List<WorldTile> tilesWithinRange = Pathfinding.GetAllVisibleTiles(startTile, range);
        foreach (WorldTile tileInRange in tilesWithinRange)
        {
            tileInRange.IsVisible = isInRange;
        }
    }

    private void InformTilesIfTheyAreWithinMovementRange(WorldTile startTile, int range, bool isInRange)
    {
        List<WorldTile> tilesWithinRange = Pathfinding.GetAllTilesWithingMovementRange(startTile, range);
        foreach (WorldTile tileInRange in tilesWithinRange)
        {
            tileInRange.IsWithinMovementRange = isInRange;
        }
        EventManager.VisibilityHasChanged();
    }



    public void SkipTurn()
    {
        InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), movementPointsLeft, false);
        movementPointsLeft = 0;
        EventManager.MaybeTurnHasEnded();
    }

    public void ReCalculateVisibility()
    {
        InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), 10, false);
        InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);
    }

    public void ResetOnEndTurn()
    {
        if(movementSpeed > 1) movementSpeed -= 1; // we want player to always have one action point
        movementPointsLeft = movementSpeed;
        visibilityRange = movementSpeed;
        InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), 10, false);
        InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), movementPointsLeft, true);
        ReCalculateVisibility();
        actionBarManager.RefreshIndicatorColors();
    }

    public int MovementPointsLeft()
    {
        return movementPointsLeft;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public GameObject GetGameObject()
    {
        return transform.gameObject;
    }

    private IEnumerator LerpThroughPath(List<WorldTile> path, int numberOfTilesToLerp)
    {
        tileUnderMe.GameObjectOnTheTile = null;
        WorldTile endTile = path[^numberOfTilesToLerp];
        tileUnderMe = endTile;
        endTile.GameObjectOnTheTile = this.gameObject;

        movementInProgress = true;
        WorldTile startingTile = GetTileUnderMyself();
        for (int i = 0; i < numberOfTilesToLerp; i++)
        {
            yield return LerpToNextTile(path, i);
            movementPointsLeft -= 1; //GetTileUnderMyself().GetTileTypeCost();
            actionBarManager.RefreshIndicatorColors();
        }

        // Refresh vision and movement range highlighting
        GetComponent<PlayerShipAudioManager>().PlayMovementSound();
        InformTilesIfTheyAreWithinVisionRange(startingTile, visibilityRange + 2, false);
        EventManager.VisibilityHasChanged();
        EventManager.MaybeTurnHasEnded();
        movementInProgress = false;
        ActivateSurroundingCubes();
    }

    private IEnumerator LerpToNextTile(List<WorldTile> path, int i)
    {
        WorldTile startingTile = GetTileUnderMyself();
        Vector3 targetPosition = path[path.Count - 1 - i].WorldPosition;
        float elapsedTime = 0;

        Vector3 velocity = Vector3.zero;
        while (elapsedTime < transitionTimeBetweenTiles)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // Rotation
            Vector3 targetDirection = (targetPosition - playerModel.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation, targetRotation, Time.deltaTime * 4);


            elapsedTime += Time.deltaTime;

            yield return null;
        }

        InformTilesIfTheyAreWithinVisionRange(startingTile, visibilityRange + 2, false);
        InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);
        transform.position = targetPosition;
    }

    private void OnEnable()
    {
        EventManager.OnVisibilityChange += ReCalculateVisibility;
        EventManager.OnTenSecondTimerEnded += ResetOnEndTurn;
        EventManager.OnTenSecondTimerEnded += StopCurrentTurn;
    }

    private void OnDisable()
    {
        EventManager.OnVisibilityChange -= ReCalculateVisibility;
        EventManager.OnTenSecondTimerEnded -= ResetOnEndTurn;
        EventManager.OnTenSecondTimerEnded -= StopCurrentTurn;
    }

    private void OnDestroy()
    {
        TurnManager.instance.playerControlledUnits.Remove(gameObject);
        MouseController.instance.selectedObject = null;
        EventManager.MaybeGameHasEnded();
    }

    protected GridLayout grid;

    public Vector3Int GetTileCoordinates()
    {
        Vector3 tilePosition = transform.position;
        tilePosition.y = 0;
        return GameTiles.instance.grid.WorldToCell(tilePosition);
    }

    public WorldTile GetTileUnderMyself()
    {
        if (transform != null)
        {
            WorldTile tileUnderCube = GameTiles.instance.GetTileByWorldPosition(transform.position);
            return tileUnderCube;
        }
        return null;
    }

    public void SetMovementPointsLeft(int pointsLeft)
    {
        movementPointsLeft = pointsLeft;
        actionBarManager.RefreshIndicatorColors();
    }

    // motion set in previous turn shouldn't continue and use points from the next one
    // or should it?
    public void StopCurrentTurn() 
    {
        StopAllCoroutines();
        // Refresh vision and movement range highlighting
        GetComponent<PlayerShipAudioManager>().PlayMovementSound();
        EventManager.VisibilityHasChanged();
        EventManager.MaybeTurnHasEnded();
        movementInProgress = false;
        ActivateSurroundingCubes();
    }
}
