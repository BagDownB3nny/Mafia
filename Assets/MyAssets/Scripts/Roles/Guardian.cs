using UnityEngine;
using Mirror;
public class Guardian : Role
{
    public override string rolePlayerInteractText => "Protect with Guardian's Sigil";

    [Server]
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        PlayerSigilManager playerSigilManager = player.GetComponent<PlayerSigilManager>();
        playerSigilManager.MarkWithSigil(Sigils.Protection);
    }
}
