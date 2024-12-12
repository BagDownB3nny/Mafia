using Mirror;
using UnityEngine;

public class PlayerActions : NetworkBehaviour
{
    private PlayerCamera playerCamera;

    public override void OnStartLocalPlayer()
    {
        playerCamera = Camera.main.GetComponent<PlayerCamera>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (EPressed())
        {
            Interactable interactable = playerCamera.GetInteratable();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private bool EPressed()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
}
