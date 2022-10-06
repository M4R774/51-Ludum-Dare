using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfNotVisible : MonoBehaviour
{
    private Transform meshChild;

    private void Start()
    {
        CheckIfVisible();

        WorldTile tileUnderMyself = GameTiles.instance.GetTileByWorldPosition(transform.position);
        tileUnderMyself.GameObjectOnTheTile = this.gameObject;
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
        meshChild = transform.GetChild(0);
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
        Vector3 worldPosition = transform.position;
        worldPosition.y = 0;
        return GameTiles.instance.grid.WorldToCell(worldPosition);
    }
}
