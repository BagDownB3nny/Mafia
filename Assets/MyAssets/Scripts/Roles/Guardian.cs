using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Guardian : Role
{
    [SyncVar]
    private uint markedPlayerNetId;
    public override string RolePlayerInteractText => "Protect with Guardian's Sigil";
    public override bool IsAbleToInteractWithPlayers => true;
    protected override List<SigilName> SigilsAbleToSee => new() { SigilName.DeathSigil };

}