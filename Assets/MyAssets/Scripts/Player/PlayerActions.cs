using Mirror;
using UnityEngine;

public class PlayerActions : NetworkBehaviour
{
    private PlayerCamera playerCamera;
    private Player player;

    public override void OnStartLocalPlayer()
    {
        playerCamera = Camera.main.GetComponent<PlayerCamera>();
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleInteractions();
        HandleShooting();
    }

    private void HandleInteractions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable interactable = playerCamera.GetInteratable();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    [Client]
    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && player.isAbleToShoot())
        {
            Debug.Log("Shots fired!");
            GameObject lookingAt = playerCamera.GetLookingAt(40.0f);
            CmdShoot(lookingAt);
        }
    }

    [Command]
    private void CmdShoot(GameObject lookingAt)
    {
        if (lookingAt != null && lookingAt.GetComponent<Shootable>() != null)
        {
            Shootable shootable = lookingAt.GetComponent<Shootable>();
            shootable.OnShot();
        }
    }

    [Client]
    public void InteractWithPlayer(Player playerInteractedWith)
    {
        CmdInteractWithPlayer(playerInteractedWith);
    }

    [Command]
    private void CmdInteractWithPlayer(Player playerInteractedWith)
    {
        Role roleScript = GetComponent<Role>();
        roleScript.InteractWithPlayer(playerInteractedWith);
    }
}
