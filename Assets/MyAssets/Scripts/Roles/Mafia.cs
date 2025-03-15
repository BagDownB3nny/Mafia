using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Mafia : Role
{
    public override string rolePlayerInteractText => "Mark for death";
    public override bool isAbleToInteractWithPlayers => true;
    protected override List<SigilName> sigilsAbleToSee => new() { SigilName.DeathSigil };

    private Player markedPlayer;

    protected override void SetNameTags()
    {
        List<Player> players = PlayerManager.instance.GetAllPlayers();
        foreach (Player player in players)
        {
            if (player.GetRole() == RoleName.Mafia)
            {
                player.SetNameTagColor(Color.red);
            }
        }
    }

    protected override void ResetNameTags()
    {
        List<Player> players = PlayerManager.instance.GetAllPlayers();
        foreach (Player player in players)
        {
            player.SetNameTagColor(Color.white);
        }
    }

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
        playerDeathSigil.Mark(player.netId);
        markedPlayer = player.GetComponentInParent<Player>();
    }
}
