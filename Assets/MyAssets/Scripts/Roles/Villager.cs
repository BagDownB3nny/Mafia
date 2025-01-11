using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Villager : Role
{
    public override string rolePlayerInteractText => null;
    public override bool isAbleToInteractWithPlayers => false;
    protected override List<Sigil> sigilsAbleToSee => new List<Sigil>();

    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.LogError("Villager cannot interact with players");
    }
}
