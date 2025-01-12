using UnityEngine;
using Mirror;

public class ProtectionSigil : Sigil
{
    private static uint markedPlayerNetId = 0;
    [Server]
    public override void Mark(uint playerNetId)
    {
        markedPlayerNetId = playerNetId;
        gameObject.SetActive(true);
        RpcSetActive(true);
        Debug.Log("Marked player with Guardian's Sigil");
        Debug.Log(markedPlayerNetId);
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

    [Server]
    public static void ResetProtectionSigil()
    {
        Player player = PlayerManager.instance.GetPlayerByNetId(markedPlayerNetId);
        if (player == null)
        {
            Debug.LogError("Player not found");
            return;
        }
        ProtectionSigil protectionSigil = player.GetComponentInChildren<ProtectionSigil>();
        if (protectionSigil == null)
        {
            Debug.LogError("ProtectionSigil not found");
            return;
        }
        protectionSigil.Unmark();
    }
}
