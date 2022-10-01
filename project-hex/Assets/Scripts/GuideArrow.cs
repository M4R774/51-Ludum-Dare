using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    public GameObject target;

    void Update()
    {
        if(target != null)
        {
            transform.LookAt(target.transform.position, -Vector3.up);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
    }
}
