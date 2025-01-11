using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;

public class SixthSense : Role
{
    public override string rolePlayerInteractText => null;
    public override bool isAbleToInteractWithPlayers => false;
    protected override List<Sigil> sigilsAbleToSee => Enum.GetValues(typeof(Sigil)).Cast<Sigil>().ToList();

    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.LogError("Sixth sense cannot interact with players");
    }
}
