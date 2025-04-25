using Mirror;

public abstract class RoleActions : NetworkBehaviour
{
    protected Player player;
    protected PlayerCamera playerCamera;


    public override void OnStartLocalPlayer()
    {
        player = GetComponentInParent<Player>();
        playerCamera = PlayerCamera.instance;
    }

    public abstract void HandleRoleSpecificActions();
}