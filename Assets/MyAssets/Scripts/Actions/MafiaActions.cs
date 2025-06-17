using UnityEngine;
using Mirror;
using System.Linq;

public class MafiaActions : RoleActions
{
    private readonly KeyCode equipGunKey = KeyCode.Q;

    [Client]
    private void Update()
    {
        if (!isLocalPlayer) return;
        HandleGunEquipping();
        HandleMafiaActions();
    }

    [Client]
    protected override void OnLookingAt(Interactable interactable)
    {
        bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Mafia);
        if (!isInteractable) return;

        if (interactable is TrapdoorTeleporter trapdoorTeleporter)
        {
            string interactableText = trapdoorTeleporter.GetInteractableText();
            if (interactableText == Interactable.notInteractableText) return;

            trapdoorTeleporter.Highlight();
            PlayerUIManager.instance.AddInteractableText(trapdoorTeleporter, interactableText);
        }
        // else if (interactable is InteractableVillageHouseMini houseMini)
        // {
        //     string interactableText = houseMini.GetInteractableText();
        //     houseMini.Highlight();
        //     PlayerUIManager.instance.AddInteractableText(houseMini, interactableText);
        // }
        else if (interactable is TargetDummy targetDummy)
        {
            string interactableText = targetDummy.GetInteractableText();
            if (interactableText == Interactable.notInteractableText) return;   

            targetDummy.Highlight();
            PlayerUIManager.instance.AddInteractableText(targetDummy, interactableText);
        }
    }
    [Client]
    protected override void OnPlayerDeath(Player player)
    {
        if (player.isLocalPlayer)
        {
            CmdToggleGun(true); // Ensure gun is unequipped if the player dies
        }
        base.OnPlayerDeath(player);
    }

    [Client]
    private void HandleMafiaActions()
    {
        Interactable interactable = playerCamera.GetInteractable();
        bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Mafia);

        if (!isInteractable) return;

        if (Input.GetKeyDown(KeyCode.R))

            if (interactable is TrapdoorTeleporter trapdoorTeleporter)
            {
                trapdoorTeleporter.Interact();
            }
            // else if (interactable is InteractableVillageHouseMini houseMini)
            // {
            //     houseMini.Interact();
            // }
            else if (interactable is TargetDummy targetDummy)
            {
                targetDummy.Interact();
            }

    }

    [Client]
    private void HandleGunEquipping()
    {
        // Only allow gun actions during night time (between 12 AM and 8 AM)
        int currentHour = TimeManagerV2.instance.currentHour;
        if (currentHour >= 8 && currentHour < 24)
        {
            // It's daytime, no gun actions allowed
            return;
        }

        // Toggle gun equip state
        if (Input.GetKeyDown(equipGunKey))
        {
            CmdToggleGun(false);
        }
    }

    [Command]
    public void CmdToggleGun(bool forceUnequip)
    {
        // Double check on server side that it's night time
        int currentHour = TimeManagerV2.instance.currentHour;
        if (currentHour >= 8 && currentHour < 24 && !forceUnequip)
        {
            // It's daytime, don't allow gun toggling
            return;
        }

        Role roleScript = player.GetRoleScript();
        if (roleScript is Mafia mafiaRole)
        {
            if (mafiaRole.HasGun() || forceUnequip)
            {
                mafiaRole.UnequipGun();
            }
            else
            {
                mafiaRole.EquipGun();
            }
        }
        return;
    }
}