using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class HouseProtectionSigil : Sigil
{
    public bool isMarked = false;
    public static List<HouseProtectionSigil> activeSigils = new List<HouseProtectionSigil>();

    [Server]
    public override void Mark(uint houseNetId)
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
