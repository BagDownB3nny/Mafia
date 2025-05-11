using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Medium : Role
{
    [Header("Medium Params")]
    public override string RolePlayerInteractText => "";
    public override bool IsAbleToInteractWithPlayers => false;

    public override string InteractWithDoorText => "";
    public override bool IsAbleToInteractWithDoors => true;

    protected override List<SigilName> SigilsAbleToSee => new() { };

    [Header("Medium internal params")]
    private bool isEnabled = false;
    private bool isCommunicating = false;
    private float timeToDeactivation;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && timeToDeactivation <= 0 && isCommunicating)
        {
            DeactivateMediumAbility();
        }
        else if (timeToDeactivation > 0 && isCommunicating)
        {
            timeToDeactivation -= Time.deltaTime;
        }
    }
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        // Implement the interaction logic for Medium with players
        Debug.Log("Medium interacting with player: " + player.name);
    }

    [Server]
    public void EnableMediumAbility()
    {
        isEnabled = true;
    }

    [Server]
    public void DisableMediumAbility()
    {
        isEnabled = false;
    }
    
    [Command]
    public void ActivateMediumAbility()
    {
        if (isEnabled)
        {
            RpcActivateMediumAbility(connectionToClient);
        } else 
        {
            PlayerUIManager.instance.RpcSetTemporaryInteractableText(connectionToClient, "You can only use the ouija board past 12am", 1.5f);
        }
    }

    [TargetRpc]
    public void RpcActivateMediumAbility(NetworkConnection target)
    {
        Player localPlayer = PlayerManager.instance.localPlayer;
        localPlayer.DisablePlayerControllersAndCamera();
        isCommunicating = true;
        timeToDeactivation = 0.5f;
        DissonanceRoomManager.instance.OnMediumActivation();
        CameraCullingMaskManager.instance.SetGhostLayerVisible();
        PlayerUIManager.instance.SetControlsText("[E] Stop communing with the dead");
    }


    [Command]
    public void DeactivateMediumAbility()
    {
        RpcDeactivateMediumAbility(connectionToClient);
    }

    [TargetRpc]
    public void RpcDeactivateMediumAbility(NetworkConnection target)
    {
        Player localPlayer = PlayerManager.instance.localPlayer;
        localPlayer.EnablePlayerControllersAndCamera();
        isCommunicating = false;
        DissonanceRoomManager.instance.OnMediumDeactivation();
        CameraCullingMaskManager.instance.SetGhostLayerInvisible();
        PlayerUIManager.instance.ClearControlsText();
    }
}