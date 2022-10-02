using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotator : MonoBehaviour
{
    private void Rotate()
    {
        Vector3 newRotaion = new();
        WindControl.instance.ChangeWindDirection();
        Debug.Log(WindControl.instance.GetWindRotation());
        newRotaion.y = WindControl.instance.GetWindRotation();
        transform.rotation = Quaternion.Euler(newRotaion);
    }

    private void OnEnable()
    {
        EventManager.OnTenSecondTimerEnded += Rotate;
    }

    private void OnDisable()
    {
        EventManager.OnTenSecondTimerEnded -= Rotate;
    }
}
