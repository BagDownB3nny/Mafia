using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Seer : Role
{
    [SyncVar]
    public Player markedPlayer;

    protected override List<LayerName> LayersAbleToSee => new() { LayerName.Seer };

    [Header("Seer internal params")]
    public bool isLookingThroughCrystalBall = false;

    [Server]
    public void RemovePreviouslyPlacedSigils()
    {
        if (markedPlayer != null)
        {
            SeeingEyeSigil markedPlayerSeeingEyeSigil = markedPlayer.GetComponentInChildren<SeeingEyeSigil>(includeInactive: true);
            markedPlayerSeeingEyeSigil.Unmark();
        }
    }
}
