using UnityEngine;
using Mirror;

public abstract class RoleActions : NetworkBehaviour
{
    protected Player player;
    protected PlayerCamera playerCamera;

    public virtual void Initialize(Player player, PlayerCamera camera)
    {
        this.player = player;
        this.playerCamera = camera;
    }

    public virtual void HandleUpdate()
    {
        HandleInteractions();
        HandleRoleSpecificActions();
    }

    protected virtual void HandleRoleSpecificActions() { }

    protected void HandleInteractions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable interactable = playerCamera.GetInteractable();
            if (interactable != null)
            {
                bool isAbleToInteractWithPlayers = player.IsAbleToInteractWithPlayers();
                if (interactable is InteractablePlayer && isAbleToInteractWithPlayers)
                {
                    CmdInteractWithPlayer(interactable.gameObject.GetComponentInParent<NetworkIdentity>());
                }
                else if (interactable is InteractableDoor)
                {
                    InteractableDoor interactableDoor = interactable as InteractableDoor;
                    HandleDoorInteraction(interactableDoor);
                }
                else if (interactable is Interactable)
                {
                    interactable.Interact();
                }
            }
        }
    }

    protected void HandleDoorInteraction(InteractableDoor door)
    {
        bool isAbleToInteractWithDoors = player.IsAbleToInteractWithDoors();
        if (door.authorisedPlayers.Contains(player.netId) || !isAbleToInteractWithDoors)
        {
            door.Interact();
        }
        else if (isAbleToInteractWithDoors)
        {
            CmdInteractWithDoor(door.GetComponentInParent<NetworkIdentity>());
        }
    }

    [Command]
    protected virtual void CmdInteractWithPlayer(NetworkIdentity playerInteractedWith)
    {
        Role roleScript = player.GetRoleScript();
        roleScript.InteractWithPlayer(playerInteractedWith);
    }

    [Command]
    protected virtual void CmdInteractWithDoor(NetworkIdentity door)
    {
        Role roleScript = player.GetRoleScript();
        roleScript.InteractWithDoor(door);
    }
}