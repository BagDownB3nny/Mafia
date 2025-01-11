using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Seer : Role
{
    [SyncVar(hook = nameof(OnMarkedPlayerNetIdChanged))]
    public uint markedPlayerNetId;

    public override string rolePlayerInteractText => "Mark with Seeing-Eye Sigil";
    public override bool isAbleToInteractWithPlayers => true;
    protected override List<Sigil> sigilsAbleToSee => new List<Sigil> { Sigil.SeeingEyeSigil };

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    public void OnEnable()
    {
        if (isLocalPlayer)
        {
            CameraCullingMaskManager.instance.SetSigilLayerVisible(Sigil.SeeingEyeSigil);
        }
    }

    public void OnDisable()
    {
        if (isLocalPlayer)
        {
            CameraCullingMaskManager.instance.SetSigilLayerInvisible(Sigil.SeeingEyeSigil);
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
        playerSeeingEyeSigil.Mark();
        markedPlayerNetId = player.netId;
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
        Debug.Log(markedPlayerSeeingEyeSigil);
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
        // Camera.main.GetComponent<PlayerCamera>().enabled = false;
    }

    [Client]
    private void EnablePlayerControllersAndCamera()
    {
        GetComponent<PlayerMovement>().enabled = true;
        Camera.main.GetComponent<MoveCamera>().enabled = true;
        // Camera.main.GetComponent<PlayerCamera>().enabled = false;
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
