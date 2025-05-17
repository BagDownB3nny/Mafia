using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Seer : Role
{
    [SyncVar]
    public Player markedPlayer;

    protected override List<SigilName> SigilsAbleToSee => new() { SigilName.SeeingEyeSigil };

    [Header("Seer internal params")]
    public bool isLookingThroughCrystalBall = false;
}
