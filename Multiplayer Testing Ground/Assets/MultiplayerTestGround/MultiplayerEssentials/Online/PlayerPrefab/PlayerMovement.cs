using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections;

public class NetworkPlayerMovement : NetworkBehaviour
{
    [Header("Mouvement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f; // Multiplicateur de vitesse en sprint
    public float jumpForce = 10f;

    [Header("Sol / Ground Check")]
    public float groundedCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpQueued;
    private bool isGrounded;
    private bool isSprinting;

    private InputActionPlayer inputActions;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        inputActions = new InputActionPlayer();
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed += _ => jumpQueued = true;
        inputActions.Player.Sprint.performed += _ => isSprinting = true;
        inputActions.Player.Sprint.canceled += _ => isSprinting = false;

        StartCoroutine(EnableGravityDelayed());
    }

    private IEnumerator EnableGravityDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        isGrounded = IsGrounded();

        MovePlayer();

        if (jumpQueued && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpQueued = false;
        }
    }

    private void MovePlayer()
    {
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 moveWorld = transform.TransformDirection(move) * currentSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveWorld.x;
        velocity.z = moveWorld.z;
        rb.linearVelocity = velocity;
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        Vector3 origin = groundCheck.position + Vector3.up * 0.1f;
        float maxDistance = groundedCheckRadius + 0.1f;

        bool grounded = Physics.SphereCast(origin, groundedCheckRadius, Vector3.down, out hit, maxDistance, groundLayer);
        Debug.DrawRay(origin, Vector3.down * maxDistance, grounded ? Color.green : Color.red);
        return grounded;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundedCheckRadius);
        }
    }
}
