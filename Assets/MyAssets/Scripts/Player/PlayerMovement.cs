using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f; // Player movement speed
    public float maxVelocityChange = 10f; // Limits how fast the Rigidbody can change velocity
    public float jumpForce = 5f; // Jump force
    public Transform orientation; // Reference to the player's orientation transform

    [Header("Ground Check")]
    public LayerMask groundLayer; // Define which layers are considered ground
    public float groundCheckHeight = 0.5f; // Height of the capsule cast (half of the player collider height)

    // Internal movement parameters

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;
    public static PlayerMovement localInstance;

    // Players have movement locked (when voting, in settings, etc.)
    private bool isLocked;
    [SerializeField] private CapsuleCollider capsuleCollider;

    void Start()
    {
        if (isLocalPlayer)
        {
            localInstance = this;
        }
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent the Rigidbody from being affected by physics rotations
        rb.maxLinearVelocity = 10f;
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (isLocked) return;

        // Perform a capsule cast to check if the player is grounded
        isGrounded = IsGrounded();

        // Get input from WASD/Arrow Keys
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical"); // W/S or Up/Down

        // Calculate movement direction based on the orientation
        moveDirection = (orientation.forward * vertical + orientation.right * horizontal).normalized;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (isLocked) return;
        MovePlayer();
    }

    public void LockPlayerMovementControls()
    {
        isLocked = true;
        // Calculate desired velocity
        Vector3 targetVelocity = Vector3.zero;

        // Calculate velocity change while preserving the Rigidbody's current Y velocity (gravity)
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);

        // Clamp the velocity change to ensure smooth movement
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxVelocityChange);

        // Apply the velocity change to the Rigidbody
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void LockPlayerMovement()
    {
        isLocked = true;
        rb.linearVelocity = Vector3.zero;
    }

    public void UnlockPlayerMovement()
    {
        isLocked = false;
    }

    private void MovePlayer()
    {
        // Calculate desired velocity
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // Calculate velocity change while preserving the Rigidbody's current Y velocity (gravity)
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);

        // Clamp the velocity change to ensure smooth movement
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxVelocityChange);

        // Apply the velocity change to the Rigidbody
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        // Apply jump force directly to the Rigidbody's Y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    private bool IsGrounded()
    {
        // Perform a capsule cast to check if the player is grounded
        float playerHeight = capsuleCollider.height;
        float radius = capsuleCollider.radius;

        // CapsuleCast from the player's position downward to check for ground
        return Physics.CapsuleCast(
            transform.position + Vector3.up * (playerHeight / 2), // Top of the capsule
            transform.position + Vector3.up * (playerHeight / 2) - Vector3.up * groundCheckHeight, // Bottom of the capsule
            radius, // Radius of the capsule
            Vector3.down, // Cast downward
            groundCheckHeight, // Max distance to check
            groundLayer // Layer mask to check against ground layers
        );
    }
}
