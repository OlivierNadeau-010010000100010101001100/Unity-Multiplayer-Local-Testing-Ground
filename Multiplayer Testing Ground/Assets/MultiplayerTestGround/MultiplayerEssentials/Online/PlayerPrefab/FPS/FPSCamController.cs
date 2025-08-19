using UnityEngine;
using Unity.Netcode;

public class FirstPersonLook : NetworkBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;

    [HideInInspector] public bool canLook = true; // 👈 Nouveau

    void Start()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!IsOwner || !canLook) return; // 👈 Empêche le mouvement de la caméra si canLook = false

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
