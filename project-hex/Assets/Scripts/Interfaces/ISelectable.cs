using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    public void Select();
    public void Unselect();
    public bool IsSelected();
    public Vector3Int GetTileCoordinates();
    public void MoveToTile(Vector3Int tileCoordinates);
}
