using UnityEngine;
using Mirror;

public class SeerSigilItem : Item
{
    public override bool isAbleToInteractWithDoors => false;
    public override bool isAbleToInteractWithPlayers => true;

    [SyncVar]
    public Player markedPlayer;

    [SyncVar]
    public bool isUsableToMarkPlayer = true;

    // Active item —> Interact with player without sigil —> Mark them —> Remove item
    // No active item —> Interact with player with sigil —> Unmark them —> Add item

    [Client]
    public override void HandlePlayerInteraction(InteractablePlayer player)
    {
        if (player == null) return;
        CmdHandlePlayerInteraction(player);
    }

    [Command]
    public void CmdHandlePlayerInteraction(InteractablePlayer interactablePlayer)
    {
        Player player = interactablePlayer.GetComponentInParent<Player>();
        if (player == null) return;

        SeeingEyeSigil seeingEyeSigil = player.GetComponentInChildren<SeeingEyeSigil>();
        if (!isUsableToMarkPlayer)
        {
            Debug.LogError("Seer is trying to mark a player while sigil is already placed");
            return;
        }

        seeingEyeSigil.Mark(player.netId);
        isUsableToMarkPlayer = false;
        markedPlayer = player;
    }
}
