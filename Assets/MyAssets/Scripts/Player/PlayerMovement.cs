using Mirror;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    // public float moveSpeed = 6f; // Player movement speed
    public float maxVelocityChange = 10f; // Limits how fast the Rigidbody can change velocity
    public float jumpForce = 5f; // Jump force
    public Transform orientation; // Reference to the player's orientation transform

    [Header("Ground Check")]
    public LayerMask groundLayer; // Define which layers are considered ground
    public float groundCheckHeight = 0.5f; // Height of the capsule cast (half of the player collider height)

    // Internal movement parameters

    private Rigidbody rb;
    private Vector3 moveDirection;
    public float bunnyHopMultiplier = 1.0f;
    public float bunnyHopThreshold = 0.1f;
    public float bunnyHopMultiplierIncrease = 1.1f;
    public float bunnyHopMultiplierMax = 2.0f;
    // private bool isGrounded;
    public static PlayerMovement localInstance;
    private bool jumpPressed = false;
    private float jumpPressedTime;

    [Header("Character controller based movement")]

    public float moveSpeed = 5f; // Movement speed

    [SerializeField] public float gravity = -15f; // Gravity strength
    [SerializeField] public float jumpHeight = 1f; // Jump height

    private CharacterController controller;
    private Vector3 velocity;

    // Players have movement locked (when voting, in settings, etc.)
    private bool isLocked;
    [SerializeField] private CapsuleCollider capsuleCollider;

    void Start()
    {
        if (isLocalPlayer)
        {
            localInstance = this;
        }
        // rb = GetComponent<Rigidbody>();
        // rb.freezeRotation = true; // Prevent the Rigidbody from being affected by physics rotations
        // rb.maxLinearVelocity = 10f;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (isLocked) return;
        // GetUserInputMoveDirectionRb();
        GetUserInputMoveDirection();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (isLocked) return;

        // MovePlayerRb();
        MovePlayer();
    }


    private void GetUserInputMoveDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical"); // W/S or Up/Down

        // Calculate movement direction based on the orientation
        moveDirection = (orientation.forward * vertical + orientation.right * horizontal).normalized;


        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
            jumpPressedTime = Time.time;
        }
    }

    private void MovePlayer()
    {
        bool isGrounded = IsGrounded();

        if (isGrounded && velocity.y < 0)
        {
            // Reset vertical velocity when grounded
            velocity.y = -2f;
        }

        if (isGrounded && !jumpPressed)
        {
            bunnyHopMultiplier = 1.0f;
        }

        // Handle jumping
        if (isGrounded && jumpPressed && Time.time - jumpPressedTime < 0.5f)
        {
            if (Time.time - jumpPressedTime < bunnyHopThreshold)
            {
                bunnyHopMultiplier *= bunnyHopMultiplierIncrease;
                bunnyHopMultiplier = Mathf.Clamp(bunnyHopMultiplier, 1.0f, bunnyHopMultiplierMax);
            }
            else
            {
                bunnyHopMultiplier = 1.0f;
            }
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }


        controller.Move(moveDirection * moveSpeed * Time.deltaTime * bunnyHopMultiplier);
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void GetUserInputMoveDirectionRb()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical"); // W/S or Up/Down

        // Calculate movement direction based on the orientation
        moveDirection = (orientation.forward * vertical + orientation.right * horizontal).normalized;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }
    }

    public void LockPlayerMovementControls()
    {
        // isLocked = true;
        // // Calculate desired velocity
        // Vector3 targetVelocity = Vector3.zero;

        // // Calculate velocity change while preserving the Rigidbody's current Y velocity (gravity)
        // Vector3 velocity = rb.linearVelocity;
        // Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);

        // // Clamp the velocity change to ensure smooth movement
        // velocityChange = Vector3.ClampMagnitude(velocityChange, maxVelocityChange);

        // // Apply the velocity change to the Rigidbody
        // rb.AddForce(velocityChange, ForceMode.VelocityChange);
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

    private void MovePlayerRb()
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

    private void MovePlayerNew()
    {

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
