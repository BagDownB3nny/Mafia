using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Villager : Role
{
    public override string rolePlayerInteractText => null;
    public override bool isAbleToInteractWithPlayers => false;
    protected override List<SigilName> sigilsAbleToSee => new List<SigilName>();

    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.LogError("Villager cannot interact with players");
    }
}
