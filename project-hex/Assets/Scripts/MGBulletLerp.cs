using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGBulletLerp : MonoBehaviour
{
    public GameObject explosion;
    private WorldTile barrageZone;

    public void SlerpToTargetAndExplode(Vector3 startWorldPosition, Vector3 targetWorldPosition)
    {
        barrageZone = GameTiles.instance.tiles[Pathfinding.gridLayout.WorldToCell(targetWorldPosition)];
        StartCoroutine(SlerpToTarget(startWorldPosition, targetWorldPosition));
    }

    private IEnumerator SlerpToTarget(Vector3 startPosition, Vector3 targetPosition)
    {
        float timeElapsed = 0;
        WorldTile startTile = GameTiles.instance.tiles[Pathfinding.gridLayout.WorldToCell(startPosition)];
        float slerpDuration = Pathfinding.GetDistanceInTiles(startTile, barrageZone);
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

        if (barrageZone.GameObjectOnTheTile != null)
        {
            Destroy(barrageZone.GameObjectOnTheTile);
            barrageZone.GameObjectOnTheTile = null;
        }

        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void DrawDangerZone(bool enableOrDisable)
    {
        barrageZone.IsInBarrageZone = enableOrDisable;
    }
}
