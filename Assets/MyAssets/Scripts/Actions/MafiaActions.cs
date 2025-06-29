using UnityEngine;
using Mirror;
using System.Linq;

public class MafiaActions : RoleActions
{

    [Client]
    private void Update()
    {
        if (!isLocalPlayer) return;
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
}