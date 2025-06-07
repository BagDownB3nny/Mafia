using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerActions : NetworkBehaviour
{
    private PlayerCamera playerCamera;
    private Player player;

    public override void OnStartLocalPlayer()
    {
        playerCamera = PlayerCamera.instance;
    }

    public void Start()
    {
        player = GetComponent<Player>();
    }

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

    void Update()
    {
        if (!isLocalPlayer) return;
        HandleInteractions();
    }

    public void OnLookingAt(Interactable interactable)
    {
        if (interactable.isLocalPlayer) return;

        bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Villager);
        if (isInteractable)
        {
            string interactableText = interactable.GetInteractableText();
            if (interactableText == "NOT INTERACTABLE") return;
            interactable.Highlight();
            PlayerUIManager.instance.AddInteractableText(interactable, interactableText);
        }
    }

    [Client]

    protected void HandleInteractions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable interactable = playerCamera.GetInteractable();
            bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Villager);
            if (isInteractable)
            {
                interactable.Interact();
                playerCamera.SetLastInteractableToNull();
            }
        }
    }
}
