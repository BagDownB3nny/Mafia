using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;

public class SixthSense : Role
{
    public override string RolePlayerInteractText => null;
    public override bool IsAbleToInteractWithPlayers => false;
    protected override List<SigilName> SigilsAbleToSee => Enum.GetValues(typeof(SigilName)).Cast<SigilName>().ToList();

    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.LogError("Sixth sense cannot interact with players");
    }
}
