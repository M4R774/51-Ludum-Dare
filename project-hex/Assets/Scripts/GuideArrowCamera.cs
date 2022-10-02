using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrowCamera : MonoBehaviour
{
    [SerializeField] Transform mainCameraRig;
    Vector3 origPos;

    void Start()
    {
        if(mainCameraRig == null) mainCameraRig = GameObject.Find("CameraRig").transform;
        origPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
    void Update()
    {
        transform.localEulerAngles = new Vector3(0, mainCameraRig.localEulerAngles.y, 0);
        //transform.localPosition = origPos;
    }
}
