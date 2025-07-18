using System.Linq;
using Mirror;
using UnityEngine;

public class GuardianActions : RoleActions
{

    [SerializeField] private Guardian guardian;

    public void Update()
    {
        if (!isLocalPlayer) return;

        HandleGuardianActions();
    }

    [Client]
    protected override void OnLookingAt(Interactable interactable)
    {
        bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Guardian);
        if (!isInteractable) return;

        if (interactable is InteractablePlayer playerInteractable)
        {
            string interactableText = "[R] Mark with Protection Sigil";
            playerInteractable.Highlight();
            PlayerUIManager.instance.AddInteractableText(playerInteractable, interactableText);
        }
        else if (interactable is InteractableDoor doorInteractable)
        {
            Door door = doorInteractable.GetComponent<Door>();
            if (!door.isOutsideDoor || door.isMafiaHouseDoor) return;

            doorInteractable.Highlight();
            string interactableText = "[R] Mark with Protection Sigil";
            PlayerUIManager.instance.AddInteractableText(doorInteractable, interactableText);
        }
    }
    [Client]
    protected override void OnPlayerDeath(Player player)
    {
        if (player.isLocalPlayer)
        {
            // If the local player dies, reset the Guardian state
            guardian.protectedHouse = null;
            guardian.protectedPlayer = null;
            guardian.RemovePreviouslyPlacedSigils();
        }
        base.OnPlayerDeath(player);
    }

    [Client]
    private void HandleGuardianActions()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Interactable interactable = playerCamera.GetInteractable();
            bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Guardian);
            if (!isInteractable) return;

            if (interactable is InteractableDoor door)
            {
                HandleDoorInteraction(door);
            }
            else if (interactable is InteractablePlayer playerInteractable)
            {
                HandlePlayerInteraction(playerInteractable);
            }
        }
    }

    [Client]
    private void HandleDoorInteraction(InteractableDoor door)
    {
        if (door == null) { Debug.LogError("Interacting with null door"); return; }
        if (door.GetComponent<Door>().isMafiaHouseDoor) return;

        CmdInteractWithDoor(door);
    }

    [Command]
    private void CmdInteractWithDoor(InteractableDoor door)
    {
        if (door == null) { Debug.LogError("[Server] Interacting with null door"); return; }

        guardian.RemovePreviouslyPlacedSigils();
        House house = door.GetComponentInParent<House>();
        house.GetComponentInChildren<HouseProtectionSigil>(includeInactive: true).Mark(house.netId);
        guardian.protectedHouse = house;
    }

    [Client]
    private void HandlePlayerInteraction(InteractablePlayer playerInteractable)
    {
        if (playerInteractable == null) { Debug.LogError("Interacting with null player"); return; }

        CmdInteractWithPlayer(playerInteractable);
    }

    [Command]
    public void CmdInteractWithPlayer(InteractablePlayer playerInteractable)
    {
        if (playerInteractable == null) { Debug.LogError("[Server] Interacting with null player"); return; }

        guardian.RemovePreviouslyPlacedSigils();
        Player player = playerInteractable.GetComponentInParent<Player>();
        player.GetComponentInChildren<PlayerProtectionSigil>(includeInactive: true).Mark(player.netId);
        guardian.protectedPlayer = player;
    }
}
