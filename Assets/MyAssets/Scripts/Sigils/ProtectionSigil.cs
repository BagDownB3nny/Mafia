using UnityEngine;
using Mirror;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ProtectionSigil : NetworkBehaviour
{
    private static uint markedPlayerNetId = 0;
    [Server]
    public void Mark(uint playerNetId)
    {
        markedPlayerNetId = playerNetId;
        gameObject.SetActive(true);
        RpcSetActive(true);
        Debug.Log("Marked player with Guardian's Sigil");
        Debug.Log(markedPlayerNetId);
    }

    [Server]
    public void Unmark()
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
    public static void ActivateAtNight()
    {
        if (markedPlayerNetId == 0)
        {
            Debug.Log("No player marked with Guardian's Sigil");
            return;
        }
        Player markedPlayer = PlayerManager.instance.GetPlayerByNetId(markedPlayerNetId);
        if (markedPlayer == null)
        {
            Debug.LogError("Player not found");
            return;
        }
        markedPlayer.house.isProtected = true;
        Debug.Log("Player protected by Guardian's Sigil");
    }
}
