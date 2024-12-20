using UnityEngine;
using Mirror;

public abstract class Role : NetworkBehaviour
{
    public abstract string rolePlayerInteractText { get; }

    public virtual void InteractWithPlayer(Player player)
    {
        Debug.Log($"Interacting with player {player.name}");
    }
}
