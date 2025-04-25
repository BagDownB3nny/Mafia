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

    protected override List<SigilName> SigilsAbleToSee => new() {};
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        // Implement the interaction logic for Medium with players
        Debug.Log("Medium interacting with player: " + player.name);
    }

    [Server]
    public void ActivateMediumAbility()
    {
        RpcActivateMediumAbility(connectionToClient);
    }

    [TargetRpc]
    public void RpcActivateMediumAbility(NetworkConnection target)
    {
        DissonanceRoomManager.instance.OnMediumActivation();
    }

    [Server]
    public void DeactivateMediumAbility()
    {
        RpcDeactivateMediumAbility(connectionToClient);
    }

    [TargetRpc]
    public void RpcDeactivateMediumAbility(NetworkConnection target)
    {
        DissonanceRoomManager.instance.OnMediumDeactivation();
    }
}