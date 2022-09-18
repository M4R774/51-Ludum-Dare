using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour, ISelectable, IHighlightable
{
    public int movementSpeed = 20;
    private GridLayout grid;
    private bool isSelected;
    private int highlightLevel;
    private Material material;

    void Start()
    {
        grid = GameTiles.instance.grid;
        isSelected = false;
        material = transform.GetComponent<Renderer>().material;
    }

    public void Select()
    {
        isSelected = true;
        DetermineEmissionAndColor();
        ColorTilesWithinMovementRange(Color.white);
    }

    public void Unselect()
    {
        isSelected = false;
        DetermineEmissionAndColor();
        ColorTilesWithinMovementRange(Color.grey);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetHighlightLevel(int levelOfHighlight)
    {
        highlightLevel = levelOfHighlight;
        DetermineEmissionAndColor();
    }

    public Vector3Int GetTileCoordinates()
    {
        Vector3 tilePosition = transform.position;
        tilePosition.y = 0;
        return grid.WorldToCell(tilePosition);
    }

    public void MoveTowardsTarget(List<WorldTile> path)
    {
        int numberOfTilesToMove = movementSpeed;
        if (path.Count < movementSpeed)
        {
            numberOfTilesToMove = path.Count;
        }

        StartCoroutine(LerpTowardsTarget(path, numberOfTilesToMove));
    }

    private void ColorTilesWithinMovementRange(Color color)
    {
        List<WorldTile> tilesWithinRange = Pathfinding.GetAllTilesWithingRange(GetTileUnderMyself(), movementSpeed);
        foreach (WorldTile tileInRange in tilesWithinRange)
        {
            tileInRange.SetColor(color);
        }
    }

    private WorldTile GetTileUnderMyself()
    {
        GameTiles.instance.tiles.TryGetValue(GetTileCoordinates(), out WorldTile tileUnderCube);
        return tileUnderCube;
    }

    private void DetermineEmissionAndColor()
    {
        if (highlightLevel >= 2 || isSelected)
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

    private IEnumerator LerpTowardsTarget(List<WorldTile> path, int numberOfTilesToLerp)
    {
        List<WorldTile> tilesWithinRangeInTheBeginning = Pathfinding.GetAllTilesWithingRange(GetTileUnderMyself(), movementSpeed);
        for (int i = 0; i < numberOfTilesToLerp; i++)
        {
            Vector3 currentPos = transform.position;
            Vector3 targetPosition = path[path.Count - 1 - i].WorldPosition;
            float elapsedTime = 0;
            float transitionTimeBetweenTiles = .3f;

            Vector3 velocity = Vector3.zero;
            float smoothTime = 0.1F;

            while (elapsedTime < transitionTimeBetweenTiles)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
                //transform.position = Vector3.Lerp(currentPos, targetPosition, (elapsedTime / transitionTimeBetweenTiles));
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            transform.position = targetPosition;
            yield return null;
        }
        foreach (WorldTile tile in tilesWithinRangeInTheBeginning)
        {
            tile.SetColor(Color.grey);
        }
        ColorTilesWithinMovementRange(Color.white);
    }
}
