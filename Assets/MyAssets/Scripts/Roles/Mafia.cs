using UnityEngine;
using Mirror;

public class Mafia : Role
{
    public override string rolePlayerInteractText => "Mark for death";

    [Server]
    public override void InteractWithPlayer(Player player)
    {
        Debug.Log($"Mafia is interacting with player {player.name}");
    }
}
