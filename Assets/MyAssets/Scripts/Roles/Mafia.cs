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

    [SyncVar]
    private bool hasGun = true;

    private GameObject localPlayerGun;
    private GameObject remotePlayerGun;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        // Get reference to gun objects
        localPlayerGun = Camera.main.transform.Find("Gun").gameObject;
        
        if (isLocalPlayer)
        {
            EnableGun();
        }
    }

    public bool HasGun()
    {
        return hasGun;
    }

    [Client]
    public void EnableGun()
    {
        if (isLocalPlayer && localPlayerGun != null)
        {
            localPlayerGun.SetActive(true);
        }
        else if (remotePlayerGun != null)
        {
            remotePlayerGun.SetActive(true);
        }
    }

    [Server]
    public void EquipGun()
    {
        hasGun = true;
        RpcEnableGun();
    }

    [ClientRpc]
    private void RpcEnableGun()
    {
        EnableGun();
    }

    [Client]
    public void DisableGun()
    {
        if (isLocalPlayer && localPlayerGun != null)
        {
            localPlayerGun.SetActive(false);
        }
        else if (remotePlayerGun != null)
        {
            remotePlayerGun.SetActive(false);
        }
    }

    [Server]
    public void UnequipGun()
    {
        hasGun = false;
        RpcDisableGun();
    }

    [ClientRpc]
    private void RpcDisableGun()
    {
        DisableGun();
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
