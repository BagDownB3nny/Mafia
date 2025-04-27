using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Villager : Role
{
    public override string RolePlayerInteractText => null;
    public override bool IsAbleToInteractWithPlayers => false;

    public override string InteractWithDoorText => null;

    public override bool IsAbleToInteractWithDoors => false;
    protected override List<SigilName> SigilsAbleToSee => new();


    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.LogError("Villager cannot interact with players");
    }
}
