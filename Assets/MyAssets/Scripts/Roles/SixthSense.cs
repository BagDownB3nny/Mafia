using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;

public class SixthSense : Role
{
    public override string RolePlayerInteractText => null;
    public override bool IsAbleToInteractWithPlayers => false;
    public override string InteractWithDoorText => null;
    public override bool IsAbleToInteractWithDoors => false;
    protected override List<SigilName> SigilsAbleToSee => Enum.GetValues(typeof(SigilName)).Cast<SigilName>().ToList();

    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.LogError("Sixth sense cannot interact with players");
    }
}
