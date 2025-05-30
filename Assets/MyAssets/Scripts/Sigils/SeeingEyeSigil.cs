using Mirror;
using UnityEngine;
using System;

public class SeeingEyeSigil : Sigil
{
    public override LayerName Layer => LayerName.Seer;
    private static uint markedPlayerNetId = 0;

    public bool isMarked = false;

    [Server]
    public override void Mark(uint playerNetId)
    {
        markedPlayerNetId = playerNetId;
        // Activating visual indicator
        gameObject.SetActive(true);
        RpcSetActive(true);
    }

    [Server]
    public override void Unmark()
    {
        markedPlayerNetId = 0;
        gameObject.SetActive(false);
        RpcSetActive(false);
    }

    [ClientRpc]
    public void RpcSetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    [Server]
    public static void ResetSeeingEyeSigil()
    {
        if (markedPlayerNetId == 0)
        {
            return;
        }
        Player player = PlayerManager.instance.GetPlayerByNetId(markedPlayerNetId);
        if (player == null)
        {
            Debug.LogError("Player not found");
            return;
        }
        SeeingEyeSigil seeingEyeSigil = player.GetComponentInChildren<SeeingEyeSigil>();
        if (seeingEyeSigil == null)
        {
            Debug.LogError("SeeingEyeSigil not found");
            return;
        }
        seeingEyeSigil.Unmark();
    }
}
