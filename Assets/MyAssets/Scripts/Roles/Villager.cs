using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Villager : Role
{
    public override string RolePlayerInteractText => null;
    public override bool IsAbleToInteractWithPlayers => false;
    protected override List<SigilName> SigilsAbleToSee => new List<SigilName>();

    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.LogError("Villager cannot interact with players");
    }
}
