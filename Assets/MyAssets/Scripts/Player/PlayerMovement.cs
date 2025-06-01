using System.ComponentModel;
using Mirror;
using UnityEngine;

public enum MovementType
{
    Normal,
    Ladder
}

// [RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    public static PlayerMovement localInstance;
    [Header("User Input")]
    float horizontal;
    float vertical;
    private bool jumpPressed = false;
    private float jumpPressedTime;

    [Header("Normal Movement Settings")]
    // public float moveSpeed = 6f; // Player movement speed
    public float maxVelocityChange = 10f; // Limits how fast the Rigidbody can change velocity
    public float jumpForce = 5f; // Jump force
    public Transform orientation; // Reference to the player's orientation transform

    [Header("Ladder Movement Settings")]
    public float ladderMoveSpeed = 3f; // Ladder movement speed

    [Header("Ground Check")]
    public LayerMask groundLayer; // Define which layers are considered ground
    public LayerMask groundLayerWhenGoThroughGroundPlayer;
    public float groundCheckHeight = 0.5f; // Height of the capsule cast (half of the player collider height)

    [Header("Bunny Hop")]
    public float bunnyHopMultiplier = 1.0f;
    public float bunnyHopThreshold = 0.1f;
    public float bunnyHopMultiplierIncrease = 1.1f;
    public float bunnyHopMultiplierMax = 2.0f;
    // private bool isGrounded;

    [Header("Character controller based movement")]

    public float moveSpeed = 5f; // Movement speed

    [SerializeField] private float gravity = -15f; // Gravity strength
    [SerializeField] private float jumpHeight = 1f; // Jump height

    private CharacterController controller;
    private Vector3 velocity;

    // Movement disabled from systems e.g. in menu, during cutscenes
    private bool isLocked;
    // Movement disabled from player interactions e.g. being executed
    private bool isFrozen;

    [SerializeField] private CapsuleCollider capsuleCollider;

    private MovementType movementType;

    [Header("Ghost Movement Settings")]
    [SerializeField] private GhostMovement ghostMovement;


    [Header("Sigils")]
    [SerializeField] private GameObject lockSigil;
    [SerializeField] private GameObject unlockSigil;

    public override void OnStartServer()
    {
        PubSub.Subscribe<PlayerDeathEventHandler>(PubSubEvent.PlayerDeath, OnDeath);
    }

    [Server]
    public void OnDeath(Player player)
    {
        RpcOnDeath(player.netId);
    }

    [TargetRpc]
    private void RpcOnDeath(uint deadPlayerNetId)
    {
        if (isLocalPlayer && netId == deadPlayerNetId)
        {
            ghostMovement.enabled = true;
            this.enabled = false;
        }
    }

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
        if (isLocked || isFrozen) return;
        // GetUserInputMoveDirectionRb();
        GetUserInputMoveDirection();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        // MovePlayerRb();
        MovePlayer();
    }


    private void GetUserInputMoveDirection()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        vertical = Input.GetAxisRaw("Vertical"); // W/S or Up/Down



        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
            jumpPressedTime = Time.time;
        }
    }

    public void ChangeToLadderMovement()
    {
        gravity = 0f;
        movementType = MovementType.Ladder;
    }

    public void ChangeToNormalMovement()
    {
        gravity = -15f;
        movementType = MovementType.Normal;
    }

    private void MovePlayer()
    {
        if (movementType == MovementType.Ladder)
        {
            MovePlayerLadder();
        }
        else if (movementType == MovementType.Normal)
        {
            MovePlayerNormal();
        }
    }

    private void MovePlayerNormal()
    {
        bool isGrounded = IsGrounded();
        Debug.Log(isGrounded);
        // Calculate movement direction based on the orientation
        Vector3 moveDirection = (orientation.forward * vertical + orientation.right * horizontal).normalized;

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

    private void MovePlayerLadder()
    {
        Vector3 moveVector = new Vector3(0, vertical, 0);
        controller.Move(moveVector * ladderMoveSpeed * Time.deltaTime);
    }

    [ClientRpc] // Possible to use [TargetRpc] if you want to lock movement for a specific player
    public void RpcLockPlayerMovement()
    {
        LockPlayerMovement();
    }

    [Client]
    public void LockPlayerMovement()
    {
        isLocked = true;
        horizontal = 0;
        vertical = 0;
    }

    [ClientRpc]
    public void RpcUnlockPlayerMovement()
    {
        UnlockPlayerMovement();
    }

    [Client]
    public void UnlockPlayerMovement()
    {
        isLocked = false;
    }

    [ClientRpc]
    public void RpcFreezePlayerMovement()
    {
        FreezePlayerMovement();
    }
    [Client]
    public void FreezePlayerMovement()
    {
        isFrozen = true;
        horizontal = 0;
        vertical = 0;
    }
    [ClientRpc]
    public void RpcUnfreezePlayerMovement()
    {
        UnfreezePlayerMovement();
    }
    [Client]
    public void UnfreezePlayerMovement()
    {
        isFrozen = false;
    }

    [ClientRpc]
    public void RpcSetLockSigilActive(bool isActive)
    {
        SetLockSigilActive(isActive);
    }

    [Client]
    public void SetLockSigilActive(bool isActive)
    {
        if (isActive)
        {
            lockSigil.SetActive(true);
        }
        else
        {
            lockSigil.SetActive(false);
            unlockSigil.SetActive(true);
            Invoke(nameof(DisableUnlockSigil), 1.5f);
        }
    }

    [Client]
    private void DisableUnlockSigil()
    {
        unlockSigil.SetActive(false);
    }

    private bool IsGrounded()
    {
        bool isGoThroughGroundPlayer = gameObject.layer == LayerName.GoThroughGroundPlayer.Index();
        LayerMask layerMask = isGoThroughGroundPlayer ? groundLayerWhenGoThroughGroundPlayer : groundLayer;

        // Perform a capsule cast to check if the player is grounded
        float playerHeight = capsuleCollider.height;
        float radius = capsuleCollider.radius;

        // CapsuleCast from the player's position downward to check for ground
        bool isGrounded = Physics.CapsuleCast(
            transform.position + Vector3.up * (playerHeight / 2), // Top of the capsule
            transform.position + Vector3.up * (playerHeight / 2) - Vector3.up * groundCheckHeight, // Bottom of the capsule
            radius, // Radius of the capsule
            Vector3.down, // Cast downward
            groundCheckHeight, // Max distance to check
            layerMask
        );

        return isGrounded;
    }
}
