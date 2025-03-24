using Mirror;
using UnityEngine;

public class GhostMovement : NetworkBehaviour
{
    [Header("Internal References")]
    private CharacterController controller;

    [Header("User input")]
    private float x;
    private float z;
    private float y;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    public void OnEnable()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Update()
    {
        if (!isLocalPlayer)
        {
            Debug.LogWarning("Not local player");
            return;
        }
        GetUserInputMoveDirection();
    }

    public void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetUserInputMoveDirection()
    {
        x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        z = Input.GetAxisRaw("Vertical"); // W/S or Up/Down
        y = GetYAxis();
    }

    private int GetYAxis()
    {
        int yAxis = 0;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            yAxis += -1;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            yAxis += 1;
        }
        return yAxis;
    }

    private void MovePlayer()
    {
        Vector3 move = transform.right * x + transform.forward * z + transform.up * y;
        controller.Move(move * Time.deltaTime * moveSpeed);
    }
}
