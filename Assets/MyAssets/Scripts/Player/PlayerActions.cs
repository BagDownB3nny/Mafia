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
        bool isAbleToInteract = GetComponent<Player>().IsAbleToInteract();
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable interactable = playerCamera.GetInteratable();
            if (interactable != null)
            {
                if (interactable is InteractablePlayer)
                {
                    CmdInteractWithPlayer(interactable.gameObject.GetComponentInParent<NetworkIdentity>());
                }
                else
                {
                    interactable.Interact();
                }
            }
        }
    }

    [Client]
    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && player.isAbleToShoot())
        {
            Vector3 currentLookingAtDirection = playerCamera.GetLookingAtDirection();
            CmdShoot(currentLookingAtDirection, transform);
        }
    }

    [Command]
    private void CmdShoot(Vector3 lookingAtDirection, Transform playerTransform)
    {
        gameObject.GetComponentInChildren<Shootable>().OnShot(connectionToClient);
        // GameObject lookingAt = PlayerCamera.GetLookingAt(lookingAtDirection, playerTransform, 40.0f);
        // Debug.Log("Looking at: " + lookingAt);
        // if (lookingAt != null && lookingAt.GetComponent<Shootable>() != null)
        // {
        //     Shootable shootable = lookingAt.GetComponent<Shootable>();
        //     if (connectionToClient == null)
        //     {
        //         Debug.LogError("Connection to client is null");
        //         return;
        //     }
        //     shootable.OnShot(connectionToClient);
        //     Debug.Log("Player was shot" + lookingAt.name);
        // }
    }

    [Command]
    private void CmdInteractWithPlayer(NetworkIdentity playerInteractedWith)
    {
        Debug.Log($"Server command, interacting with {playerInteractedWith.name}");
        Player currentPlayer = GetComponent<Player>();
        Role roleScript = currentPlayer.GetRoleScript();
        roleScript.InteractWithPlayer(playerInteractedWith);
    }
}
