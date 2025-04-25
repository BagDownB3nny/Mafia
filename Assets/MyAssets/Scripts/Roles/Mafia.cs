using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Mafia : Role
{
    public override string RolePlayerInteractText => null;
    public override bool IsAbleToInteractWithPlayers => false;

    public override string InteractWithDoorText => null;
    public override bool IsAbleToInteractWithDoors => false;
    protected override List<SigilName> SigilsAbleToSee => new();

    [SyncVar(hook = nameof(OnGunStatusChanged))]
    private bool hasGun = false;

    [Header("Gun")]

    [SerializeField] private GameObject gunVisual;

    public bool HasGun()
    {
        return hasGun;
    }

    public void OnGunStatusChanged(bool oldStatus, bool newStatus)
    {
        gunVisual.SetActive(newStatus);
    }

    [Server]
    public void EquipGun()
    {
        Debug.Log("Equipping gun");
        hasGun = true;
    }

    [Server]
    public void UnequipGun()
    {
        Debug.Log("Unequipping gun");
        hasGun = false;
    }


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
}
