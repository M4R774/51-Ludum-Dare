using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGBulletLerp : MonoBehaviour
{
    private WorldTile targetWorldTile;
    private GridLayout grid;
    private List<WorldTile> barrageZone;

    public void SlerpToTargetAndExplode(Vector3 targetWorldPosition)
    {
        barrageZone = new();
        grid = GameTiles.instance.grid;
        Vector3Int targetCellCoordinates = grid.WorldToCell(targetWorldPosition);
        targetWorldTile = GameTiles.instance.tiles[targetCellCoordinates];

        StartCoroutine(LerpToTarget());
    }

    private IEnumerator LerpToTarget()
    {
        float timeElapsed = 0;
        float lerpDuration = 1;
        Vector3 startPosition = transform.position;
        WorldTile targetTile = GetTargetTile();
        Vector3 targetPosition = targetTile.WorldPosition;
        while (timeElapsed < lerpDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition;
        // TODO: Explosion effect

        if (targetTile.GameObjectOnTheTile != null)
        {
            Destroy(targetTile.GameObjectOnTheTile);
        }
        Destroy(gameObject);
    }

    private WorldTile GetTargetTile()
    {
        barrageZone = targetWorldTile.Neighbors();
        barrageZone.Add(targetWorldTile);
        return barrageZone[Random.Range(0, barrageZone.Count)];
    }
}
