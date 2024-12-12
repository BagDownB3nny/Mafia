using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private Interactable lastInteractable;

    public Transform orientation = null;

    public void SetOrientation(Transform newOrientation)
    {
        Debug.Log("Setting orientation");
        orientation = newOrientation;
    }

    public Interactable GetInteratable()
    {
        return lastInteractable;
    }

    void Update()
    {
        HandleMoveCamera();
        HandleLookAtInteractable();
    }

    private void HandleLookAtInteractable()
    {
        GameObject lookingAt = GetLookingAt();
        if (lookingAt == null)
        {
            if (lastInteractable != null)
            {
                lastInteractable.Unhighlight();
                lastInteractable = null;
            }
            return;
        }
        else if (lookingAt != null && lookingAt.GetComponent<Interactable>() != null)
        {
            Interactable currentInteractable = lookingAt.GetComponent<Interactable>();
            if (currentInteractable == lastInteractable) return;
            if (lastInteractable != null)
            {
                lastInteractable.Unhighlight();
            }
            currentInteractable.Highlight();
            lastInteractable = currentInteractable;
        }
        else if (lastInteractable != null)
        {
            lastInteractable.Unhighlight();
        }
    }

    private void HandleMoveCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 100.0f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 100.0f;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private GameObject GetLookingAt()
    {
        RaycastHit hit;
        float maxDistance = 5.0f;
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
