using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;

public class SixthSense : Role
{
    public override string rolePlayerInteractText => null;
    public override bool isAbleToInteractWithPlayers => false;
    protected override List<SigilName> sigilsAbleToSee => Enum.GetValues(typeof(SigilName)).Cast<SigilName>().ToList();

    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.LogError("Sixth sense cannot interact with players");
    }

    protected override void SetNameTags()
    {
        // No change in name tags
    }

    protected override void ResetNameTags()
    {
        // No change in name tags
    }
}
