using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Mafia : Role
{
    protected override List<SigilName> SigilsAbleToSee => new();

    [SyncVar(hook = nameof(OnGunStatusChanged))]
    private bool hasGun = false;

    [Header("Gun")]

    [SerializeField] private GameObject remoteGunVisual;
    private GameObject localGunVisual;
    [SerializeField] private LocalPlayerGun localGunScript;

    private void Start()
    {
        localGunVisual = Camera.main.transform.Find("Gun").gameObject;
    }

    public bool HasGun()
    {
        return hasGun;
    }

    public void OnGunStatusChanged(bool oldStatus, bool newStatus)
    {
        if (isLocalPlayer)
        {
            localGunVisual.SetActive(newStatus);
            localGunScript.enabled = newStatus;
        }
        else
        {
            remoteGunVisual.SetActive(newStatus);
        }
    }

    [Server]
    public void EquipGun()
    {
        hasGun = true;
    }

    [Server]
    public void UnequipGun()
    {
        hasGun = false;
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
}
