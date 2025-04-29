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

    void Update()
    {
        if (!isLocalPlayer) return;
        HandleSettingsPress();
        HandleInteractions();
        player.GetRoleActions()?.HandleRoleSpecificActions();
    }
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

    [Client]
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
