using UnityEngine;
using Mirror;

public class Seer : Role
{
    public override string rolePlayerInteractText => "Mark with Seeing-Eye Sigil";

    [Server]
    public override void InteractWithPlayer(Player player)
    {
        Debug.Log($"Seer is interacting with player {player.name}");
    }
}
