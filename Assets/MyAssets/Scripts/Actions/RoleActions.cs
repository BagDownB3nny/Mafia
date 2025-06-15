using Mirror;

public abstract class RoleActions : NetworkBehaviour
{
    protected Player player;
    protected PlayerCamera playerCamera;

    public void Start()
    {
        player = GetComponentInParent<Player>();
        playerCamera = PlayerCamera.instance;
    }
    public virtual void OnEnable()
    {
        if (!isLocalPlayer) return;
        PubSub.Subscribe<NewInteractableLookedAtEventHandler>(PubSubEvent.NewInteractableLookedAt, OnLookingAt);
        PubSub.Subscribe<PlayerDeathEventHandler>(PubSubEvent.PlayerDeath, OnPlayerDeath);
    }
    public virtual void OnDisable()
    {
        if (!isLocalPlayer) return;
        PubSub.Unsubscribe<NewInteractableLookedAtEventHandler>(PubSubEvent.NewInteractableLookedAt, OnLookingAt);
        PubSub.Unsubscribe<PlayerDeathEventHandler>(PubSubEvent.PlayerDeath, OnPlayerDeath);
    }

    protected abstract void OnLookingAt(Interactable interactable);

    [Client]
    protected virtual void OnPlayerDeath(Player player)
    {
        if (player.isLocalPlayer)
        {
            PlayerUIManager.instance.ClearControlsText();
        }
    }
}