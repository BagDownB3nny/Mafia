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

    [Header("Seer internal params")]
    private bool isLookingThroughCrystalBall = false;
    private float timeToDeactivation;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && timeToDeactivation <= 0 && isLookingThroughCrystalBall)
        {
            DeactivateSeerAbility();
        }
        else if (timeToDeactivation > 0 && isLookingThroughCrystalBall)
        {
            timeToDeactivation -= Time.deltaTime;
        }
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
    public void ActivateSeerAbility()
    {
        Player markedPlayer = PlayerManager.instance.GetPlayerByNetId(markedPlayerNetId);
        if (markedPlayer == null)
        {
            // PlayerUIManager.instance.SetTemporaryInteractableText("No player marked with Seeing-Eye Sigil", 1.5f);
            string text = "No player marked with Seeing-Eye Sigil";
            PlayerUIManager.instance.SetTemporaryInteractableText(text, 2f);
            return;
        }


        SeeingEyeSigil markedPlayerSeeingEyeSigil = markedPlayer.GetComponentInChildren<SeeingEyeSigil>(includeInactive: true);
        if (markedPlayerSeeingEyeSigil == null)
        {
            Debug.LogError("Player does not have a seeing-eye sigil");
            return;
        }
        markedPlayerSeeingEyeSigil.Activate();
        Player localPlayer = PlayerManager.instance.localPlayer;
        localPlayer.DisablePlayerControllersAndCamera();
        isLookingThroughCrystalBall = true;
        timeToDeactivation = 0.5f;
        PlayerUIManager.instance.SetControlsText("[E] Exit Crystal Ball");
    }

    [Client]
    private void DeactivateSeerAbility()
    {
        Camera.main.GetComponentInChildren<FollowSeeingEyeSigil>(includeInactive: true).enabled = false;
        isLookingThroughCrystalBall = false;
        Player localPlayer = PlayerManager.instance.localPlayer;
        localPlayer.EnablePlayerControllersAndCamera();
        PlayerUIManager.instance.ClearControlsText();
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
