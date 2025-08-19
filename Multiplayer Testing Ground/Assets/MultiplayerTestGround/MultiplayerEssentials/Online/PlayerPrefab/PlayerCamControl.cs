using UnityEngine;

public class PlayerCameraControl : MonoBehaviour
{
    public Transform target;
    public float sensitivity = 3f;

    private float minDistance = 2f;
    private float maxDistance = 10f;
    public float minY = -20f;
    public float maxY = 80f;


    private float distance = 7f;
    private float zoomSpeed = 8f;
    private float rotX = 0f;
    private float rotY = 0f;

    private bool cursorLocked = true;

    void Start()
    {
        rotX = 32f;
        rotY = 0f;
        distance = 7f;
        LockCursor(true);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockCursor(false);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            LockCursor(true);
        }

        if (!cursorLocked) return;

        if (target == null) return;

        rotX += Input.GetAxis("Mouse X") * sensitivity;
        rotY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotY = Mathf.Clamp(rotY, minY, maxY);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        Vector3 direction = rotation * new Vector3(0, 0, -distance);
        transform.position = target.position + direction;
        transform.LookAt(target);
    }

    private void LockCursor(bool locked)
    {
        cursorLocked = locked;
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
