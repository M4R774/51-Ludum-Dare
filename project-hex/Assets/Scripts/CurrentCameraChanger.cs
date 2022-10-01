using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentCameraChanger : MonoBehaviour
{
    
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera skyCamera;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameras();
        }
    }

    void ToggleCameras()
    {
        playerCamera.enabled = !playerCamera.enabled;
        skyCamera.enabled = !skyCamera.enabled;
    }
}
