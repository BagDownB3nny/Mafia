using UnityEngine;
using Mirror;

public class Seer : Role
{
    [SyncVar]
    private uint markedPlayerNetId;

    public override string rolePlayerInteractText => "Mark with Seeing-Eye Sigil";

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
        GetComponent<PlayerMovement>().enabled = false;
        Camera.main.GetComponent<MoveCamera>().enabled = false;
        // Camera.main.GetComponent<PlayerCamera>().enabled = false;
    }
}
