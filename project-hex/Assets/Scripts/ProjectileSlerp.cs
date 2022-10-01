using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlerp : MonoBehaviour
{
    public void SlerpToTargetAndExplode(Vector3 targetWorldPosition)
    {
        StartCoroutine(LerpToTarget(targetWorldPosition));

        // TODO: Explosion effect, do dagame etc. 
    }

    private IEnumerator LerpToTarget(Vector3 targetPosition)
    {
        float timeElapsed = 0;
        float lerpDuration = 3;

        while (timeElapsed < lerpDuration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition;
        Destroy(gameObject);
    }

}
