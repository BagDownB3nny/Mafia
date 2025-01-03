using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Guardian : Role
{
    [SyncVar]
    private uint markedPlayerNetId;
    public override string rolePlayerInteractText => "Protect with Guardian's Sigil";
    public override bool isAbleToInteractWithPlayers => true;
    protected override List<Sigils> sigilsAbleToSee => new List<Sigils> { Sigils.DeathSigil };

    public void OnEnable()
    {
        if (isLocalPlayer)
        {
            CameraCullingMaskManager.instance.SetSigilLayerVisible(Sigils.ProtectionSigil);
        }
    }

    public void OnDisable()
    {
        if (isLocalPlayer)
        {
            CameraCullingMaskManager.instance.SetSigilLayerInvisible(Sigils.ProtectionSigil);
        }
    }

    [Server]
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        Player markedPlayer = PlayerManager.instance.GetPlayerByNetId(markedPlayerNetId);
        if (markedPlayer != null)
        {
            ProtectionSigil markedPlayerGuardianSigil = markedPlayer.GetComponentInChildren<ProtectionSigil>(includeInactive: true);
            markedPlayerGuardianSigil.Unmark();
        }

        ProtectionSigil playerGuardianSigil = player.GetComponentInChildren<ProtectionSigil>(includeInactive: true);
        if (playerGuardianSigil == null)
        {
            Debug.LogError("Player does not have a guardian sigil");
            return;
        }
        markedPlayerNetId = player.netId;
        playerGuardianSigil.Mark(markedPlayerNetId);
    }
}
