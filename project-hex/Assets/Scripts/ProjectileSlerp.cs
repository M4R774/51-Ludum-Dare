using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlerp : MonoBehaviour
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

        StartCoroutine(LerpToTarget(targetWorldPosition));

        // TODO: Explosion effect, do dagame etc. 
    }

    private IEnumerator LerpToTarget(Vector3 targetPosition)
    {
        float timeElapsed = 0;
        float lerpDuration = 5;
        Vector3 startPosition = transform.position;
        DrawDangerZone(true);
        while (timeElapsed < lerpDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        DrawDangerZone(false);
        transform.position = targetPosition;
        Destroy(gameObject);
    }

    private void DrawDangerZone(bool enableOrDisable)
    {
        barrageZone = targetWorldTile.Neighbors();
        barrageZone.Add(targetWorldTile);

        foreach (WorldTile barrageTile in barrageZone)
        {
            barrageTile.IsInBarrageZone = enableOrDisable;
        }
    }
}
