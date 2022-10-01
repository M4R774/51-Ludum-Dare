using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : AbstractObjectInWorldSpace, ISelectable, IHighlightable
{
    public int visibilityRange;
    public int movementPointsLeft;
    public int movementSpeed;
    public bool isPlayable;

    private bool isSelected;
    private int highlightLevel;
    private Material material;
    private bool movementInProgress;
    private WorldTile tileUnderMe;

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
        InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);

        tileUnderMe = GetTileUnderMyself();
        tileUnderMe.GameObjectOnTheTile = transform.gameObject;

        movementPointsLeft = movementSpeed;
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
        InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);
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
    }

    private IEnumerator LerpToNextTile(List<WorldTile> path, int i)
    {
        WorldTile startingTile = GetTileUnderMyself();
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

        InformTilesIfTheyAreWithinVisionRange(startingTile, visibilityRange, false);
        InformTilesIfTheyAreWithinVisionRange(GetTileUnderMyself(), visibilityRange, true);
        transform.position = targetPosition;
    }

    private void OnEnable()
    {
        EventManager.OnVisibilityChange += ReCalculateVisibility;
        EventManager.OnEndTurn += ResetOnEndTurn;
    }

    private void OnDisable()
    {
        EventManager.OnVisibilityChange -= ReCalculateVisibility;
        EventManager.OnEndTurn -= ResetOnEndTurn;
    }
}
