using UnityEngine;
using Mirror;
using System.Linq;

public class MafiaActions : RoleActions
{
    private readonly KeyCode equipGunKey = KeyCode.Q;

    public void OnEnable()
    {
        if (!isLocalPlayer || !isClient) return;

        PubSub.Subscribe<NewInteractableLookedAtEventHandler>(PubSubEvent.NewInteractableLookedAt, OnLookingAt);
    }

    public void OnDisable()
    {
        if (!isLocalPlayer || !isClient) return;

        PubSub.Unsubscribe<NewInteractableLookedAtEventHandler>(PubSubEvent.NewInteractableLookedAt, OnLookingAt);
    }

    [Client]
    private void Update()
    {
        if (!isLocalPlayer) return;
        HandleGunEquipping();
        HandleMafiaActions();
    }

    [Client]
    public void OnLookingAt(Interactable interactable)
    {
        bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Mafia);
        if (!isInteractable) return;

        if (interactable is TeleporterRune teleporterRune)
        {
            string interactableText = teleporterRune.GetInteractableText();
            teleporterRune.Highlight();
            PlayerUIManager.instance.AddInteractableText(teleporterRune, interactableText);
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
            targetDummy.Highlight();
            PlayerUIManager.instance.AddInteractableText(targetDummy, interactableText);
        }
    }

    [Client]
    private void HandleMafiaActions()
    {
        Interactable interactable = playerCamera.GetInteractable();
        bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Mafia);

        if (!isInteractable) return;

        if (Input.GetKeyDown(KeyCode.R))

            if (interactable is TeleporterRune teleporterRune)
            {
                teleporterRune.Interact();
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
            CmdToggleGun();
        }
    }

    [Command]
    public void CmdToggleGun()
    {
        // Double check on server side that it's night time
        int currentHour = TimeManagerV2.instance.currentHour;
        if (currentHour >= 8 && currentHour < 24)
        {
            // It's daytime, don't allow gun toggling
            return;
        }

        Role roleScript = player.GetRoleScript();
        if (roleScript == null)
        {
            Debug.LogError("Role script is null");
            return;
        }
        if (roleScript is Mafia mafiaRole)
        {
            if (mafiaRole.HasGun())
            {
                mafiaRole.UnequipGun();
            }
            else
            {
                mafiaRole.EquipGun();
            }
        }
    }
}