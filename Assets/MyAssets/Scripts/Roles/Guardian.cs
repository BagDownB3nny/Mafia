using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Guardian : Role
{
    [SyncVar]
    private uint markedPlayerNetId;
    public override string rolePlayerInteractText => "Protect with Guardian's Sigil";
    public override bool isAbleToInteractWithPlayers => true;
    protected override List<SigilName> sigilsAbleToSee => new List<SigilName> { SigilName.DeathSigil };

    protected override void SetNameTags()
    {
        // No change in name tags
    }

    protected override void ResetNameTags()
    {
        // No change in name tags
    }
}