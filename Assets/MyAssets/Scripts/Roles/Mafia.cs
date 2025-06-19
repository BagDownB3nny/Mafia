using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Mafia : Role
{
    [SerializeField] private PlayerInventory playerInventory;
    protected override List<LayerName> LayersAbleToSee => new() {LayerName.Mafia};
    public override void OnEnable()
    {
        base.OnEnable();
        if (!isLocalPlayer) return;
        playerInventory.CmdAddGunToInventory();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (!isLocalPlayer) return;
        playerInventory.CmdRemoveGunFromInventory();
    }

    [Client]
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
    public void ServerDisableMafiaInventorySlots()
    {
        playerInventory.ServerDisableMafiaInventorySlots();
    }
}
