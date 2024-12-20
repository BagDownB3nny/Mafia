using UnityEngine;
using Mirror;
public class Guardian : Role
{
    public override string rolePlayerInteractText => "Protect with Guardian's Sigil";

    [Server]
    public override void InteractWithPlayer(Player player)
    {
        Debug.Log($"Guardian is interacting with player {player.name}");
    }
}
