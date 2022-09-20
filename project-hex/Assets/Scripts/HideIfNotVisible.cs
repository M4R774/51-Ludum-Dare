using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfNotVisible : MonoBehaviour
{
    private GridLayout grid;
    private MeshRenderer renderer;

    private void Start()
    {
        renderer = transform.GetComponent<MeshRenderer>();
        grid = GameTiles.instance.grid;
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
            renderer.enabled = true;
        }
        else
        {
            renderer.enabled = false;
        }
    }

    public Vector3Int GetTileCoordinates()
    {
        Vector3 tilePosition = transform.position;
        tilePosition.y = 0;
        return grid.WorldToCell(tilePosition);
    }
}
