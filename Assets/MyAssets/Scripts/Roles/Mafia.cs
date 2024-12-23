using UnityEngine;
using Mirror;

public class Mafia : Role
{
    public override string rolePlayerInteractText => "Mark for death";

    [Server]
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        PlayerSigilManager playerSigilManager = player.GetComponent<PlayerSigilManager>();
        playerSigilManager.MarkWithSigil(Sigils.Death);
    }
}
