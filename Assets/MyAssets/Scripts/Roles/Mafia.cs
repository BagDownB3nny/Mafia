using UnityEngine;
using Mirror;

public class Mafia : Role
{
    public override string rolePlayerInteractText => "Mark for death";

    private Player markedPlayer;

    [Server]
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        if (markedPlayer != null)
        {
            PlayerSigilManager markedPlayerSigilManager = markedPlayer.GetComponentInChildren<PlayerSigilManager>();
            markedPlayerSigilManager.UnmarkWithSigil(Sigils.Death);
        }
        PlayerSigilManager playerSigilManager = player.GetComponentInChildren<PlayerSigilManager>();
        playerSigilManager.MarkWithSigil(Sigils.Death);
        markedPlayer = player.GetComponent<Player>();
    }
}
