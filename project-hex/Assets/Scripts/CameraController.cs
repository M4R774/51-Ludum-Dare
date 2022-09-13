using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public bool mouseMoveEnabled;
    public float screenEdgeBorderThickness;
    public float cameraSpeed;
    public float cameraSmoothing;
    public Vector3 zoomSpeed;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) ||
            Input.mousePosition.y >= Screen.height - screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition += cameraSpeed * Time.deltaTime * transform.forward;
        }
        if (Input.GetKey(KeyCode.S) ||
            Input.mousePosition.y <= screenEdgeBorderThickness && mouseMoveEnabled)
        {
            newPosition -= cameraSpeed * Time.deltaTime * transform.forward;
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
            newRotation *= Quaternion.Euler(Vector3.up);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(-Vector3.up);
        }

        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomSpeed;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomSpeed;
        }


        transform.SetPositionAndRotation(
            Vector3.Lerp(transform.position, newPosition, Time.deltaTime * cameraSmoothing), 
            Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * cameraSmoothing));
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition, 
            newZoom, 
            Time.deltaTime * cameraSmoothing);
    }
}

