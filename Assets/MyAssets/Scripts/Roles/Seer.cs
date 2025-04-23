using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Seer : Role
{
    [SyncVar(hook = nameof(OnMarkedPlayerNetIdChanged))]
    public uint markedPlayerNetId;

    public override string RolePlayerInteractText => "Mark with Seeing-Eye Sigil";
    public override bool IsAbleToInteractWithPlayers => true;

    public override string InteractWithDoorText => null;
    public override bool IsAbleToInteractWithDoors => false;
    protected override List<SigilName> SigilsAbleToSee => new() { SigilName.SeeingEyeSigil };

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        roleActions = gameObject.AddComponent<VillagerActions>();
    }

    [Server]
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Player markedPlayer = PlayerManager.instance.GetPlayerByNetId(markedPlayerNetId);
        // Remove previously placed seeing-eye sigil since only one death mark can be placed per mafia
        if (markedPlayer != null)
        {
            SeeingEyeSigil markedPlayerSeeingEyeSigil = markedPlayer.GetComponentInChildren<SeeingEyeSigil>(includeInactive: true);
            markedPlayerSeeingEyeSigil.Unmark();
        }

        // Place new seeing-eye sigil on player
        SeeingEyeSigil playerSeeingEyeSigil = player.GetComponentInChildren<SeeingEyeSigil>(includeInactive: true);
        if (playerSeeingEyeSigil == null)
        {
            Debug.LogError("Player does not have a seeing-eye sigil");
            return;
        }
        markedPlayerNetId = player.netId;
        playerSeeingEyeSigil.Mark(markedPlayerNetId);
    }

    [Client]
    public void LookThroughCrystalBall()
    {
        Player markedPlayer = PlayerManager.instance.GetPlayerByNetId(markedPlayerNetId);
        if (markedPlayer == null)
        {
            // PlayerUIManager.instance.SetTemporaryInteractableText("No player marked with Seeing-Eye Sigil", 1.5f);
            string debugText = $"markedPlayerNetId: {markedPlayerNetId}, markedPlayer: {markedPlayer}";
            PlayerUIManager.instance.SetTemporaryInteractableText(debugText, 1.5f);
            return;
        }


        SeeingEyeSigil markedPlayerSeeingEyeSigil = markedPlayer.GetComponentInChildren<SeeingEyeSigil>(includeInactive: true);
        if (markedPlayerSeeingEyeSigil == null)
        {
            Debug.LogError("Player does not have a seeing-eye sigil");
            return;
        }
        DisablePlayerControllersAndCamera();
        markedPlayerSeeingEyeSigil.Activate();
        markedPlayerSeeingEyeSigil.OnDeactivate += EnablePlayerControllersAndCamera;
        Debug.Log("Looking through crystal ball");
    }

    [Client]
    private void DisablePlayerControllersAndCamera()
    {
        GetComponent<PlayerMovement>().enabled = false;
        Camera.main.GetComponent<MoveCamera>().enabled = false;
    }

    [Client]
    private void EnablePlayerControllersAndCamera()
    {
        GetComponent<PlayerMovement>().enabled = true;
        Camera.main.GetComponent<MoveCamera>().enabled = true;
    }

    private void OnMarkedPlayerNetIdChanged(uint oldMarkedPlayerNetId, uint newMarkedPlayerNetId)
    {
        Player markedPlayer = PlayerManager.instance.GetPlayerByNetId(newMarkedPlayerNetId);
        Debug.Log(newMarkedPlayerNetId);
        if (markedPlayer != null)
        {
            Debug.Log($"Player {markedPlayer.name} marked with Seeing-Eye Sigil");
        }
    }
}
