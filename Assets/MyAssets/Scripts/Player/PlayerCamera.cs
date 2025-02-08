using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private Interactable lastInteractable;

    public Transform orientation = null;

    // Cursor mode is for when the player is in a menu (e.g. settings)
    public bool isCursorMode = false;
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

    public Interactable GetInteratable()
    {
        return lastInteractable;
    }

    public Shootable GetShootable()
    {
        GameObject lookingAt = GetLookingAt(40.0f);
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
        HandleLookAtInteractable();
    }

    private void HandleLookAtInteractable()
    {
        GameObject lookingAt = GetLookingAt(5.0f);
        if (lookingAt == null || lookingAt.GetComponent<Interactable>() == null)
        {
            if (lastInteractable != null)
            {
                lastInteractable.OnUnhover();
                // lastInteractable.Unhighlight();
                lastInteractable = null;
            }
            return;
        }
        // Is looking at an interactable
        else if (lookingAt != null && lookingAt.GetComponent<Interactable>() != null)
        {
            Interactable currentInteractable = lookingAt.GetComponent<Interactable>();

            // If the current interactable is the same as the last one, do nothing
            if (currentInteractable == lastInteractable) return;
            if (lastInteractable != null)
            {
                lastInteractable.OnUnhover();
                // lastInteractable.Unhighlight();
            }
            currentInteractable.OnHover();
            // currentInteractable.Highlight();
            lastInteractable = currentInteractable;
        }
    }

    public Vector3 GetLookingAtDirection()
    {
        return transform.forward;
    }

    private void HandleMoveCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 300.0f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 300.0f;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public static GameObject GetLookingAt(Vector3 lookingAtDirection, Transform origin, float maxDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin.position, lookingAtDirection, out hit, maxDistance))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public GameObject GetLookingAt(float maxDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public void EnterFPSMode()
    {
        Debug.Log("Entering fps mode");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorMode = false;
    }

    public void EnterCursorMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorMode = true;
    }
}
