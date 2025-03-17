using UnityEngine;
using Mirror;

public class Billboard : MonoBehaviour
{
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;

        if (playerCamera == null) {
            playerCamera = FindFirstObjectByType<Camera>();
        }
    }

    void LateUpdate()
    {
        // Check for camera in the case it was destroyed and re-created
        if (playerCamera == null) {
            playerCamera = Camera.main;
            if (playerCamera == null) {
                Debug.LogWarning("Billboard: no camera found");
            }
        }

        // Rotate to face the camera
        transform.LookAt(transform.position + playerCamera.transform.forward);
    }
}
