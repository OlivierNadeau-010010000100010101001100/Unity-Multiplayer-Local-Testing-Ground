using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [HideInInspector] public bool canMove = true;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    void Update()
    {
        if (!canMove) return;

        Move();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        Vector3 moveWorld = transform.TransformDirection(move) * moveSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveWorld.x;
        velocity.z = moveWorld.z;
        rb.linearVelocity = velocity;
    }
}
