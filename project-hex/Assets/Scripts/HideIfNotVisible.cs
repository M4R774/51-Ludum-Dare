using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfNotVisible : MonoBehaviour
{
    private GridLayout grid;
    private Transform meshChild;

    private void Start()
    {
        meshChild = transform.GetChild(0);
        grid = GameTiles.instance.grid;
        CheckIfVisible();

        WorldTile tileUnderMyself = GameTiles.instance.GetTileByWorldPosition(transform.position);
        tileUnderMyself.GameObjectOnTheTile = transform.gameObject;
    }

    private void OnEnable()
    {
        EventManager.OnVisibilityChange += CheckIfVisible;
    }

    private void OnDisable()
    {
        EventManager.OnVisibilityChange -= CheckIfVisible;
    }

    public void CheckIfVisible()
    {
        GameTiles.instance.tiles.TryGetValue(GetTileCoordinates(), out WorldTile tileUnderMyself);
        if (tileUnderMyself.IsVisible)
        {
            meshChild.gameObject.SetActive(true);
        }
        else
        {
            meshChild.gameObject.SetActive(false);
        }
    }

    public Vector3Int GetTileCoordinates()
    {
        Vector3 tilePosition = transform.position;
        tilePosition.y = 0;
        return grid.WorldToCell(tilePosition);
    }
}
