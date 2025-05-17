using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Guardian : Role
{

    [Header("Marked player / house")]

    [SyncVar]
    private uint markedPlayerNetId;

    [SyncVar]
    private uint markedHouseNetId;

    protected override List<SigilName> SigilsAbleToSee => new() { SigilName.ProtectionSigil };

    [Header("Guardian internal params")]

    [SyncVar]
    public Player protectedPlayer;

    [SyncVar]
    public House protectedHouse;

    [Server]
    public void RemovePreviouslyPlacedSigils()
    {
        if (protectedPlayer != null)
        {
            PlayerProtectionSigil markedPlayerProtectionSigil = protectedPlayer.GetComponentInChildren<PlayerProtectionSigil>(includeInactive: true);
            markedPlayerProtectionSigil.Unmark();
        }
        if (protectedHouse != null)
        {
            HouseProtectionSigil markedHouseProtectionSigil = protectedHouse.GetComponentInChildren<HouseProtectionSigil>(includeInactive: true);
            markedHouseProtectionSigil.Unmark();
        }
    }
}