using UnityEngine;
using Unity.Netcode;

public class SimplePlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        if(!IsOwner) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;

        Vector3 move = new Vector3(moveX, 0f, moveZ).normalized;

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}
