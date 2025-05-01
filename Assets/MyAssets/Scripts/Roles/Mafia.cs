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

    [SerializeField] private GameObject remoteGunVisual;
    private GameObject localGunVisual;
    [SerializeField] private LocalPlayerGun localGunScript;

    private void Start()
    {
        localGunVisual = Camera.main.transform.Find("Gun").gameObject;
    }

    public override void InteractWithPlayer(NetworkIdentity player)
    {
        // Implement the interaction logic for Mafia with players
        Debug.Log("Mafia interacting with player: " + player.name);
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
            Debug.Log("Local gun status changed: " + newStatus);
        }
        else
        {
            remoteGunVisual.SetActive(newStatus);
        }
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


    [Client]
    protected override void SetNameTags()
    {
        Debug.Log("setting name tags as mafia");
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
