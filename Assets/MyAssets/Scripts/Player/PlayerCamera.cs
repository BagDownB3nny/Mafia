using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private Interactable lastInteractable;

    public Transform orientation = null;

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
            Debug.Log("Looking at " + lookingAt.name);
            if (lastInteractable != null)
            {
                lastInteractable.OnUnhover();
                // lastInteractable.Unhighlight();
            }
            Debug.Log("Highlighting " + currentInteractable.GetRat());
            currentInteractable.OnHover();
            // currentInteractable.Highlight();
            lastInteractable = currentInteractable;
        }
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EnterCursorMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
