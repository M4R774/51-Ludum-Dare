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

    private ActionBarManager actionBarManager;

    void Start()
    {
        movementInProgress = false;
        grid = GameTiles.instance.grid;
        isSelected = false;
        material = transform.GetComponent<Renderer>().material;
        if (!isPlayable)
        {
            material.color = Color.blue;
        }
        else
        {
            InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);
        }

        tileUnderMe = GetTileUnderMyself();
        tileUnderMe.GameObjectOnTheTile = transform.gameObject;

        movementPointsLeft = movementSpeed;
        actionBarManager = ActionBarManager.instance;

        if(playerModel == null) playerModel = this.gameObject;
    }

    public int MovementSpeed
    {
        get { return movementPointsLeft; }
        //set { movementSpeed = value; }
    }

    public void Select()
    {
        if (isPlayable)
        {
            isSelected = true;
            actionBarManager.SetVisible(movementPointsLeft);
            DetermineEmissionAndColor();
            InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), movementPointsLeft, true);
        }

    }

    public void Unselect()
    {
        isSelected = false;
        DetermineEmissionAndColor();
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
        DetermineEmissionAndColor();
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
        if (isPlayable) {
            InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);
        }
    }

    public void ResetOnEndTurn()
    {
        movementPointsLeft = movementSpeed;
        if (isSelected)
        {
            InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), movementPointsLeft, true);
        }

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

    private void DetermineEmissionAndColor()
    {
        if (!isPlayable) {
            return;
        }
        else if (highlightLevel >= 2 || isSelected)
        {
            material.SetColor("_EmissionColor", Color.yellow);
        }
        else if (highlightLevel == 1)
        {
            material.SetColor("_EmissionColor", Color.grey);
        }
        else
        {
            material.SetColor("_EmissionColor", Color.black);
        }
    }

    private IEnumerator LerpThroughPath(List<WorldTile> path, int numberOfTilesToLerp)
    {
        tileUnderMe.GameObjectOnTheTile = null;
        WorldTile endTile = path[^numberOfTilesToLerp];
        tileUnderMe = endTile;
        endTile.GameObjectOnTheTile = transform.gameObject;

        movementInProgress = true;
        WorldTile startingTile = GetTileUnderMyself();
        for (int i = 0; i < numberOfTilesToLerp; i++)
        {
            yield return LerpToNextTile(path, i);
            movementPointsLeft -= 1; //GetTileUnderMyself().GetTileTypeCost();
            actionBarManager.SetVisible(movementPointsLeft);
        }

        // Refresh vision and movement range highlighting
        InformTilesIfTheyAreWithinMovementRange(startingTile, movementSpeed, false);
        InformTilesIfTheyAreWithinVisionRange(startingTile, visibilityRange, false);
        if (isSelected)
        {
            InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), movementPointsLeft, true);
        }
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

        InformTilesIfTheyAreWithinVisionRange(startingTile, visibilityRange, false);
        InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);
        transform.position = targetPosition;
    }

    private void AddMovementPoint()
    {
        if (movementPointsLeft < 10) {
            movementPointsLeft = 10;
            InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), 5, false);
            InformTilesIfTheyAreWithinMovementRange(GetTileUnderMyself(), movementPointsLeft, true);
            actionBarManager.SetVisible(movementPointsLeft);
        }
    }

    private void OnEnable()
    {
        EventManager.OnVisibilityChange += ReCalculateVisibility;
        EventManager.OnEndTurn += ResetOnEndTurn;
        EventManager.OnTenSecondTimerEnded += AddMovementPoint;
    }

    private void OnDisable()
    {
        EventManager.OnVisibilityChange -= ReCalculateVisibility;
        EventManager.OnEndTurn -= ResetOnEndTurn;
        EventManager.OnTenSecondTimerEnded -= AddMovementPoint;
    }

    private void OnDestroy()
    {
        TurnManager.instance.playerControlledUnits.Remove(gameObject);
        EventManager.MaybeGameHasEnded();
    }
}
