using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractObjectInWorldSpace : MonoBehaviour
{
    protected GridLayout grid;

    public Vector3Int GetTileCoordinates()
    {
        Vector3 tilePosition = transform.position;
        tilePosition.y = 0;
        return grid.WorldToCell(tilePosition);
    }

    public WorldTile GetTileUnderMyself()
    {
        WorldTile tileUnderCube = GameTiles.instance.GetTileByWorldPosition(transform.position);
        return tileUnderCube;
    }
}
