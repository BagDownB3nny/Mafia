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

    [Header("Guardian Params")]

    public override string RolePlayerInteractText => "Protect with Protection Sigil";
    public override bool IsAbleToInteractWithPlayers => true;

    public override string InteractWithDoorText => "Protect with Protection Sigil";

    public override bool IsAbleToInteractWithDoors => true;
    protected override List<SigilName> SigilsAbleToSee => new() { SigilName.ProtectionSigil };

    [Header("Guardian internal params")]
    private Player protectedPlayer;
    private House protectedHouse;

    [Server]
    public override void InteractWithPlayer(NetworkIdentity playerNetworkIdentity)
    {
        RemovePreviouslyPlacedSigils();
        Player player = playerNetworkIdentity.GetComponent<Player>();
        player.GetComponentInChildren<PlayerProtectionSigil>(includeInactive: true).Mark(player.netId);
        protectedPlayer = player;
    }

    [Server]
    public override void InteractWithDoor(NetworkIdentity door)
    {
        Door doorComponent = door.GetComponent<Door>();
        if (doorComponent.isOutsideDoor)
        {
            // Cannot interact with inside door
            return;
        }

        RemovePreviouslyPlacedSigils();
        House house = door.GetComponentInParent<House>();
        house.GetComponentInChildren<HouseProtectionSigil>(includeInactive: true).Mark(house.netId);
        protectedHouse = house;
    }

    [Server]
    private void RemovePreviouslyPlacedSigils()
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