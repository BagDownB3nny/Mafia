using System.Linq;
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

    [Header("Camera modes")]

    // Cursor mode is for when the player is in a menu (e.g. settings)
    public bool isCursorMode = false;

    // Spectator mode is for when the player is spectating
    public bool isSpectatorMode = false;

    public static PlayerCamera instance;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        EnterFPSMode();
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
        if (isCursorMode) return;
        HandleMoveCamera();
        if (isSpectatorMode) return;
        HandleLookAtInteractable();
    }

    private void HandleLookAtInteractable()
    {
        GameObject lookingAt = GetFilteredLookingAt<Interactable>(5.0f);

        bool isLookingAtInteractable = lookingAt != null && lookingAt.GetComponentInParent<Interactable>() != null;

        if (isLookingAtInteractable)
        {

            if (lastInteractable != null)
            {
                SetLastInteractableToNull();
            }

            Interactable currentInteractable = lookingAt.GetComponentInParent<Interactable>();
            PubSub.Publish(PubSubEvent.NewInteractableLookedAt, currentInteractable);
            lastInteractable = currentInteractable;
        }
        else if (!isLookingAtInteractable && lastInteractable != null)
        {
            SetLastInteractableToNull();
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
        if (Physics.Raycast(originPosition, lookingAtDirection, out hit, maxDistance))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public static GameObject GetLookingAt(Vector3 lookingAtDirection, Vector3 originPosition, float maxDistance)
    {
        // lookingAtDirection and originPosition are both client-side
        // The raycast is done on the server, so the hit object might be a different object based on server position
        RaycastHit hit;
        if (Physics.Raycast(originPosition, lookingAtDirection, out hit, maxDistance))
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
        if (Physics.Raycast(originPosition, lookingAtDirection, out hit, maxDistance))
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
        bool hasHit = Physics.Raycast(originPosition, lookingAtDirection, out RaycastHit hit, maxDistance);
        if (!hasHit)
        {
            return null;
        }
        Debug.Log($"Hit: {hit.collider.gameObject.name} at distance {hit.distance}");
        T component = hit.collider.GetComponentInParent<T>();
        if (component != null)
        {
            return component.gameObject;
        }
        else
        {
            return null;
        }
        // RaycastHit[] hits = Physics.RaycastAll(transform.position, lookingAtDirection, maxDistance);
        // hits = hits.Where(h => h.collider.gameObject.GetComponentInParent<T>() != null).OrderBy(h => h.distance).ToArray();

        // if (hits.Length > 0)
        // {
        //     return hits[0].collider.gameObject;
        // }

        // return null;
    }

    public void EnterFPSMode()
    {
        Debug.Log("Entering fps mode");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorMode = false;
        isSpectatorMode = false;
    }

    public void EnterCursorMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorMode = true;
    }

    public void ExitCursorMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorMode = false;
    }


    public void EnterSpectatorMode()
    {
        Debug.Log("Entering spectator mode");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorMode = false;
        isSpectatorMode = true;
        if (lastInteractable != null)
        {
            lastInteractable.Unhighlight();
            PlayerUIManager.instance.RemoveInteractableText(lastInteractable);
            lastInteractable = null;
        }
    }
}
