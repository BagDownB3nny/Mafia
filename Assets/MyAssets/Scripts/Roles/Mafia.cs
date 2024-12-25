using UnityEngine;
using Mirror;

public class Mafia : Role
{
    public override string rolePlayerInteractText => "Mark for death";

    private Player markedPlayer;

    [Server]
    public override void InteractWithPlayer(NetworkIdentity player)
    {
        // Remove previously placed death sigil since only one death mark can be placed per mafia
        if (markedPlayer != null)
        {
            DeathSigil markedPlayerDeathSigil = markedPlayer.GetComponentInChildren<DeathSigil>(includeInactive: true);
            markedPlayerDeathSigil.Unmark();
        }

        // Place new death sigil on player
        DeathSigil playerDeathSigil = player.GetComponentInChildren<DeathSigil>(includeInactive: true);
        if (playerDeathSigil == null)
        {
            Debug.LogError("Player does not have a death sigil");
            return;
        }
        playerDeathSigil.Mark();
        markedPlayer = player.GetComponentInParent<Player>();
    }
}
