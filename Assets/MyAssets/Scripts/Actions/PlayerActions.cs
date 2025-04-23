using Mirror;
using UnityEngine;

public class PlayerActions : NetworkBehaviour
{
    private RoleActions currentRoleActions;
    private PlayerCamera playerCamera;
    private Player player;

    public override void OnStartLocalPlayer()
    {
        playerCamera = PlayerCamera.instance;
        player = GetComponent<Player>();
        InitializeRoleActions();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        HandleSettingsPress();
        if (currentRoleActions != null)
        {
            currentRoleActions.HandleUpdate();
        }
    }

    private void InitializeRoleActions()
    {
        // Create the appropriate role actions based on player's role
        currentRoleActions = player.GetRole() switch
        {
            RoleName.Mafia => gameObject.AddComponent<MafiaActions>(),
            _ => gameObject.AddComponent<VillagerActions>(),
        };
        currentRoleActions.Initialize(player, playerCamera);
    }

    // Called when player's role changes
    public void OnRoleChanged(RoleName newRole)
    {
        if (currentRoleActions != null)
        {
            Destroy(currentRoleActions);
        }
        InitializeRoleActions();
    }
    protected void HandleSettingsPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingsManager settingsManager = SettingsManager.instance;
            if (settingsManager != null)
            {
                settingsManager.ToggleSettings();
            }
        }
    }
}
