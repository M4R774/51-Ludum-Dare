using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public bool mouseMoveEnabled;
    public float screenEdgeBorderThickness;
    public float cameraSpeed;
    public float cameraSmoothing;
    public Vector3 zoomSpeed;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newEulerRotation;
    private Vector3 newZoom;

    private void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        newEulerRotation = transform.eulerAngles;
    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.W) ||
            Input.mousePosition.y >= Screen.height - screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition += cameraSpeed * Time.deltaTime * Vector3.Normalize(new Vector3(transform.forward.x, 0, transform.forward.z));
        }
        if (Input.GetKey(KeyCode.S) ||
            Input.mousePosition.y <= screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition -= cameraSpeed * Time.deltaTime * Vector3.Normalize(new Vector3(transform.forward.x, 0, transform.forward.z));
        }
        if (Input.GetKey(KeyCode.A) ||
            Input.mousePosition.x <= screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition -= cameraSpeed * Time.deltaTime * transform.right;
        }
        if (Input.GetKey(KeyCode.D) ||
            Input.mousePosition.x >= Screen.width - screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition += cameraSpeed * Time.deltaTime * transform.right;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            newEulerRotation += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newEulerRotation -= new Vector3(0, 1, 0);
        }

        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomSpeed;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomSpeed;
        }
        newZoom += zoomSpeed * Input.mouseScrollDelta.y * 4;

        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = (rotateStartPosition - rotateCurrentPosition)/5;

            newEulerRotation += new Vector3(difference.y, -difference.x, 0);

            rotateStartPosition = rotateCurrentPosition;
        }

        // Position
        newPosition.y = 0;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * cameraSmoothing);

        // Rotation
        Vector3 idealEuler;
        idealEuler.x = newEulerRotation.x;
        idealEuler.y = newEulerRotation.y;
        idealEuler.z = 0;
        var qr = Quaternion.Euler(idealEuler);
        transform.rotation = Quaternion.Lerp(transform.rotation, qr, Time.deltaTime * cameraSmoothing);


        // Zoom
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition, 
            newZoom, 
            Time.deltaTime * cameraSmoothing);


    }
}

