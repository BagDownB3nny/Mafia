using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerProtectionSigil : Sigil
{
    public bool isMarked = false;
    public static List<PlayerProtectionSigil> activeSigils = new();

    [Server]
    public override void Mark(uint playerNetId)
    {
        gameObject.SetActive(true);
        isMarked = true;
        activeSigils.Add(this);
        RpcSetActive(true);
    }

    [Server]
    public override void Unmark()
    {
        gameObject.SetActive(false);
        isMarked = false;
        activeSigils.Remove(this);
        RpcSetActive(false);
    }
    [ClientRpc]
    public void RpcSetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    [Server]
    public static void ResetProtectionSigils()
    {
        foreach (Sigil sigil in activeSigils)
        {
            sigil.Unmark();
        }
    }
}
