using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public bool enableWASD = true;

    [Header("Zoom Settings")]
    public bool enableZoom = true;
    public float scrollSpeed = 5f;
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 20f;

    [Header("Movement Bounds (XZ)")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    private Camera cam;
    private float fixedY;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (!cam.orthographic)
            Debug.LogWarning("CameraController conçu pour une caméra Orthographique !");
        fixedY = transform.position.y;
    }

    void Update()
    {
        HandlePan();
        HandleZoom();
    }

    void HandlePan()
    {
        Vector3 pos = transform.position;
        float dt = Time.deltaTime;

        // directions sur XZ
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        Vector3 move = Vector3.zero;

        // clavier/WASD
        if (Input.GetKey(KeyCode.UpArrow) || (enableWASD && Input.GetKey(KeyCode.W))) move += forward;
        if (Input.GetKey(KeyCode.DownArrow) || (enableWASD && Input.GetKey(KeyCode.S))) move -= forward;
        if (Input.GetKey(KeyCode.RightArrow) || (enableWASD && Input.GetKey(KeyCode.D))) move += right;
        if (Input.GetKey(KeyCode.LeftArrow) || (enableWASD && Input.GetKey(KeyCode.A))) move -= right;

        // bord écran
        Vector3 ms = Input.mousePosition;
        if (ms.y >= Screen.height - panBorderThickness) move += forward;
        if (ms.y <= panBorderThickness) move -= forward;
        if (ms.x >= Screen.width - panBorderThickness) move += right;
        if (ms.x <= panBorderThickness) move -= right;

        // déplacement + clamp sur XZ
        pos += move.normalized * panSpeed * dt;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        pos.y = fixedY;
        transform.position = pos;
    }

    void HandleZoom()
    {
        if (!enableZoom) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - scroll * scrollSpeed,
                minOrthoSize,
                maxOrthoSize
            );
        }
    }
}
