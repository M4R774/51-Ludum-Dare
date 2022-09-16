using UnityEngine;

// CameraController is responsible for moving the camera around. Currently you can
// move camera with mouse or WASD + Q/E + R/F keys. mouseMoveEnabled variable determines
// if the camera should move then mouse is on edge of the screen. 
public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public bool mouseMoveEnabled;
    public float screenEdgeBorderThickness;
    public float cameraSpeed;
    public float cameraSmoothing;
    public Vector3 zoomSpeed;

    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;
    private Vector3 newPosition;
    private Vector3 newEulerRotation;
    private Vector3 newZoom;

    private void Start()
    {
        newPosition = transform.position;
        newZoom = cameraTransform.localPosition;
        newEulerRotation = transform.eulerAngles;
    }

    void LateUpdate()
    {
        // WASD
        if (Input.GetKey(KeyCode.W) ||
            Input.mousePosition.y >= Screen.height - screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition += cameraSpeed * Time.deltaTime * Vector3.Normalize(new Vector3(transform.forward.x, 0, transform.forward.z));
        }
        if (Input.GetKey(KeyCode.A) ||
            Input.mousePosition.x <= screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition -= cameraSpeed * Time.deltaTime * transform.right;
        }
        if (Input.GetKey(KeyCode.S) ||
            Input.mousePosition.y <= screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition -= cameraSpeed * Time.deltaTime * Vector3.Normalize(new Vector3(transform.forward.x, 0, transform.forward.z));
        }
        if (Input.GetKey(KeyCode.D) ||
            Input.mousePosition.x >= Screen.width - screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition += cameraSpeed * Time.deltaTime * transform.right;
        }
        
        // Rotation
        if (Input.GetKey(KeyCode.Q))
        {
            newEulerRotation += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newEulerRotation -= new Vector3(0, 1, 0);
        }

        // Mouse rotation and zoom
        if (Time.timeScale > 0)
        {
            // Mouse drag rotation
            if (Input.GetMouseButtonDown(2))
            {
                rotateStartPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                rotateCurrentPosition = Input.mousePosition;
                Vector3 difference = (rotateStartPosition - rotateCurrentPosition) / 5;

                newEulerRotation += new Vector3(difference.y, -difference.x, 0);

                rotateStartPosition = rotateCurrentPosition;
            }

            // Zoom
            if (Input.GetKey(KeyCode.R))
            {
                newZoom += zoomSpeed;
            }
            if (Input.GetKey(KeyCode.F))
            {
                newZoom -= zoomSpeed;
            }
            newZoom += zoomSpeed * Input.mouseScrollDelta.y * 4;
        }

        // Position
        newPosition.y = 0;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * cameraSmoothing);

        // Rotation
        Vector3 idealEuler;
        idealEuler.x = Mathf.Clamp(newEulerRotation.x, -40, 45);
        idealEuler.y = newEulerRotation.y;
        idealEuler.z = 0;
        var qr = Quaternion.Euler(idealEuler);
        transform.rotation = Quaternion.Lerp(transform.rotation, qr, Time.deltaTime * cameraSmoothing);

        // Zoom
        newZoom.y = Mathf.Clamp(newZoom.y, 4, 100);
        newZoom.z = -newZoom.y;
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition, 
            newZoom, 
            Time.deltaTime * cameraSmoothing);
    }
}

