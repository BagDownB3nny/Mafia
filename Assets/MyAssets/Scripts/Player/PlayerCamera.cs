using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    [Header("User input")]
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    public Interactable lastInteractable;

    [Header("Player info")]
    public Transform orientation = null;


    [Header("Ignore raycast layers")]
    public static LayerMask ignoreRaycastLayers;
    private MoveCamera moveCamera;
    public static PlayerCamera instance;

    private CameraMode _currentMode;
    private CameraMode previousMode;
    private readonly Dictionary<CameraMode, List<CameraMode>> transitions = new()
    {
        { CameraMode.FirstPerson, new List<CameraMode> { CameraMode.FirstPerson, CameraMode.Cursor, CameraMode.Spectator, CameraMode.CrystalBall } },
        { CameraMode.Cursor, new List<CameraMode> { CameraMode.FirstPerson, CameraMode.Spectator, CameraMode.CrystalBall } },
        { CameraMode.Spectator, new List<CameraMode> { CameraMode.Cursor } },
        { CameraMode.CrystalBall, new List<CameraMode> { CameraMode.FirstPerson, CameraMode.Cursor, CameraMode.Spectator } }
    };
    public CameraMode CurrentMode
    {
        get => _currentMode;
        private set
        {
            if (_currentMode == value) return;

            var oldMode = _currentMode;
            _currentMode = value;
            OnCameraModeChanged(oldMode, value);
        }
    }

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        moveCamera = Camera.main.GetComponent<MoveCamera>();
        ignoreRaycastLayers = LayerMask.GetMask(
            LayerName.LocalPlayer.ToString(),
            LayerName.IgnoreRaycast.ToString(),
            LayerName.GoThroughGroundPlayer.ToString()
        );
        // Explicit FPS mode initialization to ensure the camera starts in the correct mode
        CurrentMode = CameraMode.FirstPerson;
        moveCamera.SetToPlayerCameraPosition();
        OnCameraModeChanged(CurrentMode, CameraMode.FirstPerson);
    }

    public bool CanTransitionTo(CameraMode from, CameraMode to)
    {
        // Safeguard against invalid transitions
        return transitions.ContainsKey(from) && transitions[from].Contains(to);
    }

    private void OnCameraModeChanged(CameraMode oldMode, CameraMode newMode)
    {
        // Get death status once
        bool isPlayerDead = NetworkClient.localPlayer?.GetComponent<PlayerDeath>()?.isDead ?? false;

        // If player is dead, only allow Spectator or Cursor modes
        if (isPlayerDead && newMode != CameraMode.Spectator && newMode != CameraMode.Cursor)
        {
            // Prevent switching to FirstPerson or CrystalBall modes if the player is dead
            Debug.LogWarning($"Dead player cannot enter {newMode} mode. Forcing Spectator mode.");
            _currentMode = CameraMode.Spectator;
            return;
        }
        Debug.Log($"Transitioning camera mode from {oldMode} to {newMode}");

        if (!CanTransitionTo(oldMode, newMode))
        {
            Debug.LogWarning($"Invalid camera mode transition from {oldMode} to {newMode}. Current mode remains {CurrentMode}.");
            return;
        }

        switch (newMode)
        {
            case CameraMode.FirstPerson when !isPlayerDead:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case CameraMode.Cursor:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case CameraMode.Spectator:
            case CameraMode.CrystalBall when !isPlayerDead:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SetLastInteractableToNull();
                break;
            default:
                Debug.LogWarning($"Unhandled camera mode: {newMode}");
                break;
        }
    }

    public void SetOrientation(Transform newOrientation)
    {
        orientation = newOrientation;
    }

    public Interactable GetInteractable()
    {
        return lastInteractable;
    }

    public Shootable GetShootable()
    {
        GameObject lookingAt = GetFilteredLookingAt<Shootable>(40.0f);
        if (lookingAt != null && lookingAt.GetComponent<Shootable>() != null)
        {
            return lookingAt.GetComponent<Shootable>();
        }
        return null;
    }

    void Update()
    {
        if (orientation == null) return;

        // If the player is in cursor mode, do nothing
        if (CurrentMode == CameraMode.Cursor) return;
        HandleMoveCamera();
        if (CurrentMode == CameraMode.FirstPerson)
        {
            HandleLookAtInteractable();
        }
    }

    private void HandleLookAtInteractable()
    {
        GameObject lookingAt = GetFilteredLookingAt<Interactable>(5.0f);

        bool isLookingAtInteractable = lookingAt != null && lookingAt.GetComponentInParent<Interactable>() != null;
        SetLastInteractableToNull();

        if (isLookingAtInteractable)
        {
            Interactable currentInteractable = lookingAt.GetComponentInParent<Interactable>();
            PubSub.Publish(PubSubEvent.NewInteractableLookedAt, currentInteractable);
            lastInteractable = currentInteractable;
        }
    }

    public void SetLastInteractableToNull()
    {
        if (lastInteractable != null)
        {
            lastInteractable.Unhighlight();
            PlayerUIManager.instance.RemoveInteractableText(lastInteractable);
            lastInteractable = null;
        }
    }

    public Vector3 GetLookingAtDirection()
    {
        return transform.forward;
    }

    private void HandleMoveCamera()
    {
        float mouseSensitivity = MouseSenseSlider.mouseSensitivity;
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 150.0f * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 150.0f * mouseSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public static GameObject GetLookingAt(Vector3 lookingAtDirection, Transform origin, float maxDistance)
    {
        RaycastHit hit;
        // Offset the origin position to avoid hitting the player
        Vector3 originPosition = origin.position;
        if (Physics.Raycast(originPosition, lookingAtDirection, out hit, maxDistance, ~ignoreRaycastLayers))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public static GameObject GetLookingAt(Vector3 lookingAtDirection, Vector3 originPosition, float maxDistance)
    {
        // lookingAtDirection and originPosition are both client-side
        // The raycast is done on the server, so the hit object might be a different object based on server position
        if (Physics.Raycast(originPosition, lookingAtDirection, out RaycastHit hit, maxDistance, ~ignoreRaycastLayers))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public GameObject GetLookingAt(float maxDistance)
    {
        RaycastHit hit;
        // Offset the origin position to avoid hitting the player
        Vector3 lookingAtDirection = transform.forward;
        Vector3 originPosition = transform.position;
        if (Physics.Raycast(originPosition, lookingAtDirection, out hit, maxDistance, ~ignoreRaycastLayers))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public GameObject GetFilteredLookingAt<T>(float maxDistance) where T : MonoBehaviour
    {
        // Offset the origin position to avoid hitting the player
        Vector3 lookingAtDirection = transform.forward;
        Vector3 originPosition = transform.position;
        bool hasHit = Physics.Raycast(originPosition, lookingAtDirection, out RaycastHit hit, maxDistance, ~ignoreRaycastLayers);
        if (!hasHit)
        {
            return null;
        }
        T component = hit.collider.GetComponentInParent<T>();
        if (component != null)
        {
            return component.gameObject;
        }
        else
        {
            return null;
        }
    }

    public void EnterFPSMode()
    {
        CurrentMode = CameraMode.FirstPerson;
        moveCamera.SetToPlayerCameraPosition();
    }

    public void EnterCursorMode()
    {
        previousMode = CurrentMode;
        CurrentMode = CameraMode.Cursor;
    }

    public void EnterSpectatorMode()
    {
        if (CurrentMode == CameraMode.Cursor)
        {
            // Store the spectator mode and stay in cursor mode
            previousMode = CameraMode.Spectator;
            return;
        }
        previousMode = CurrentMode;
        CurrentMode = CameraMode.Spectator;
    }

    public void EnterCrystalBallMode(Transform position)
    {
        moveCamera.SetCameraPosition(position);
        PlayerMovement.localInstance.FreezePlayerMovement();
        if (CurrentMode == CameraMode.Cursor)
        {
            // Store the crystal ball mode and set the camera position
            // Set the previous mode to allow exiting later
            previousMode = CameraMode.CrystalBall;
            return;
        }
        previousMode = CurrentMode;
        CurrentMode = CameraMode.CrystalBall;
    }

    public void ExitCursorMode()
    {
        bool isPlayerDead = NetworkClient.localPlayer?.GetComponent<PlayerDeath>()?.isDead ?? false;
        if (isPlayerDead)
        {
            EnterSpectatorMode();
        }
        else
        {
            CurrentMode = previousMode;
        }
    }

    public void ExitCrystalBallMode()
    {
        CurrentMode = previousMode;
    }
}
