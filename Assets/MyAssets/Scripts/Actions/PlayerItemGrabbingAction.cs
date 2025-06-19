using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerItemGrabbingAction : NetworkBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isLocalPlayer || !isClient) return;
        PubSub.Subscribe<NewInteractableLookedAtEventHandler>(PubSubEvent.NewInteractableLookedAt, OnLookingAt);
    }

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

    public void OnDestroy()
    {
        if (isClient)
        {
            PubSub.Unsubscribe<NewInteractableLookedAtEventHandler>(PubSubEvent.NewInteractableLookedAt, OnLookingAt);
        }
    }
    public void Update()
    {
        if (!isLocalPlayer) return;
        HandleInteractions();
    }

    public void OnLookingAt(Interactable interactable)
    {
        if (interactable.isLocalPlayer) return;

        bool isObtainableItem = interactable is ObtainableItem;
        bool canRoleInteract = CanRoleInteract(interactable); 
        if (isObtainableItem && canRoleInteract)
        {
            string interactableText = interactable.GetInteractableText();
            if (interactableText == "NOT INTERACTABLE") return;
            interactable.Highlight();
            PlayerUIManager.instance.AddInteractableText(interactable, interactableText);
        }
    }

    public void HandleInteractions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable interactable = PlayerCamera.instance.GetInteractable();
            if (interactable == null) return;
            if (interactable is ObtainableItem obtainableItem)
            {
                HandleInteractWithObtainableItem(obtainableItem);
            }
        }
    }

    public bool CanRoleInteract(Interactable interactable)
    {
        Player player = GetComponentInParent<Player>();
        RoleName[] rolesThatCanInteract = interactable.GetRolesThatCanInteract();
        return rolesThatCanInteract.Contains(player.role);
    }

    [Client]
    public void HandleInteractWithObtainableItem(ObtainableItem obtainableItem)
    {
        bool hasInventorySpace = playerInventory.HasInventorySpace();
        bool playerHasItem = playerInventory.HasItem(obtainableItem.item);
        if (obtainableItem.isInWorld && hasInventorySpace)
        {
            CmdAddItemToInventory(obtainableItem);
            obtainableItem.CmdRemoveFromWorld();
        } else if (!obtainableItem.isInWorld && playerHasItem)
        {
            CmdRemoveItemFromInventory(obtainableItem);
            obtainableItem.CmdAddToWorld();
        }
    }

    [Command]
    public void CmdAddItemToInventory(ObtainableItem obtainableItem)
    {
        Items item = obtainableItem.item;
        if (item == Items.None) 
        {
            Debug.LogError("Items is None");
        }
        playerInventory.ServerAddItem(item);
    }

    [Command]
    public void CmdRemoveItemFromInventory(ObtainableItem obtainableItem)
    {
        Items item = obtainableItem.item;
        if (item == Items.None) 
        {
            Debug.LogError("Items is None");
        }
        playerInventory.ServerRemoveItem(item);
    }
}
