using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlerp : MonoBehaviour
{
    public GameObject explosion;
    private WorldTile targetWorldTile;
    private GridLayout grid;
    private List<WorldTile> barrageZone;

    public void SlerpToTargetAndExplode(Vector3 startWorldPosition, Vector3 targetWorldPosition)
    {
        barrageZone = new();
        grid = GameTiles.instance.grid;
        Vector3Int targetCellCoordinates = grid.WorldToCell(targetWorldPosition);
        targetWorldTile = GameTiles.instance.tiles[targetCellCoordinates];

        StartCoroutine(SlerpToTarget(startWorldPosition, targetWorldPosition));
    }

    private IEnumerator SlerpToTarget(Vector3 startPosition, Vector3 targetPosition)
    {
        float timeElapsed = 0;
        float slerpDuration = 2;
        Debug.Log(startPosition);
        DrawDangerZone(true);
        while (timeElapsed < slerpDuration)
        {
            // The center of the arc
            Vector3 center = (startPosition + targetPosition) * 0.5F;

            // move the center a bit upwards to make the arc vertical
            center -= new Vector3(0, 1, 0);

            // Interpolate over the arc relative to center
            Vector3 startRelCenter = startPosition - center;
            Vector3 endRelCenter = targetPosition - center;

            // The fraction of the animation that has happened so far is
            // equal to the elapsed time divided by the desired time for
            // the total journey.
            float fracComplete = timeElapsed / slerpDuration;

            transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete);
            transform.position += center;
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        DrawDangerZone(false);
        transform.position = targetPosition;
        foreach(WorldTile dangerTile in barrageZone)
        {
            if (dangerTile.GameObjectOnTheTile != null)
            {
                Destroy(dangerTile.GameObjectOnTheTile);
            }
        }
        Instantiate(explosion, transform.position, transform.rotation);
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
