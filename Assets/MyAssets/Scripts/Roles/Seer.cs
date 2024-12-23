using UnityEngine;
using Mirror;

public class Seer : Role
{
    private GameObject seeingEyeSigilPrefab;

    public override string rolePlayerInteractText => "Mark with Seeing-Eye Sigil";

    [Server]
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        PlayerSigilManager playerSigilManager = player.GetComponent<PlayerSigilManager>();
        playerSigilManager.MarkWithSigil(Sigils.SeeingEye);
    }
}
