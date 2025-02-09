using System;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;



public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnRoleChanged))]
    public RoleName role;

    [SyncVar(hook = nameof(OnUsernameChanged))]
    public string steamUsername;
    [SerializeField] private TMP_Text playerUIPrefab;
    [SerializeField] private Transform cameraTransform;

    [SyncVar(hook = nameof(OnGunStatusChanged))]
    private bool hasGun;

    // When a player equips a gun, the client will see a different gun compared to the other players 
    // Imagine a game like valorant, where the player is able to see his own gun differently from how he sees 
    // the guns of other players.
    // Allows for a better view of the gun for both player and other players
    [SerializeField] private GameObject localPlayerGun;
    [SerializeField] private GameObject remotePlayerGun;

    [SyncVar]
    public House house;

    Dictionary<Enum, Role> roleScripts;

    public void Start()
    {
        GetRoleScripts();
    }

    public override void OnStartLocalPlayer()
    {
        StartCamera();
        GetSteamUsername();

        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.localPlayer = this;
        }

        localPlayerGun = Camera.main.transform.Find("Gun").gameObject;
    }

    [Client]
    private void GetRoleScripts()
    {
        roleScripts = new Dictionary<Enum, Role>
        {
            {RoleName.Seer, gameObject.GetComponentInChildren<Seer>(includeInactive: true)},
            {RoleName.Guardian, gameObject.GetComponentInChildren<Guardian>(includeInactive: true)},
            {RoleName.Mafia, gameObject.GetComponentInChildren<Mafia>(includeInactive: true)},
            {RoleName.SixthSense, gameObject.GetComponentInChildren<SixthSense>(includeInactive: true)},
            {RoleName.Villager, gameObject.GetComponentInChildren<Villager>(includeInactive: true)}
        };
    }

    [Client]
    private void StartCamera()
    {
        Camera.main.transform.GetComponent<MoveCamera>().SetCameraPosition(cameraTransform);
        PlayerCamera.instance.SetOrientation(this.transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
    }

    [Client]
    public void GetSteamUsername()
    {
        // TODO - Uncomment this when Steamworks.NET is implemented
        // if (SteamManager.Initialized)
        // {
        //     steamUsername = SteamFriends.GetPersonaName();
        //     CmdUpdateSteamUsername(steamUsername);
        // }
        // else
        // {
        //     steamUsername = "Player " + Random.Range(0, 1000);
        //     CmdUpdateSteamUsername(steamUsername);
        // }
        steamUsername = "Player " + UnityEngine.Random.Range(0, 1000);
        CmdUpdateSteamUsername(steamUsername);
        playerUIPrefab.text = steamUsername;
    }

    public Role GetRoleScript()
    {
        return roleScripts[role];
    }

    [Server]
    public void SetRole(RoleName newRole)
    {
        role = newRole;
        EnableRoleScript(newRole);

        house.SpawnRoom(newRole);
    }

    [Client]
    public RoleName GetRole()
    {
        return role;
    }

    private void EnableRoleScript(RoleName newRole)
    {
        roleScripts[newRole].enabled = true;
    }

    private void DisableRoleScriptsExcept(RoleName roleToKeep)
    {
        foreach (RoleName role in Enum.GetValues(typeof(RoleName)))
        {
            if (role != roleToKeep)
            {
                roleScripts[role].enabled = false;
            }
        }
    }

    [Command]
    private void CmdUpdateSteamUsername(string newUsername)
    {
        steamUsername = newUsername;
        if (PlayerManager.instance)
        {
            PlayerManager.instance.AddPlayer(steamUsername, netId);
        }
    }

    [Client]
    private void OnUsernameChanged(string oldUsername, string newUsername)
    {
        playerUIPrefab.text = newUsername;
    }

    [Client]
    public void OnRoleChanged(RoleName oldRole, RoleName newRole)
    {
        if (isLocalPlayer)
        {
            PlayerUIManager.instance.SetRoleText(newRole);
        }
        EnableRoleScript(newRole);
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

    public void OnGunStatusChanged(bool oldStatus, bool newStatus)
    {
        if (isLocalPlayer)
        {
            localPlayerGun.SetActive(newStatus);
        }
        else
        {
            remotePlayerGun.SetActive(newStatus);
        }
    }

    public bool isAbleToShoot()
    {
        return hasGun;
    }

    public bool IsAbleToInteractWithPlayers()
    {
        if (GetRoleScript() == null)
        {
            return false;
        }
        return GetRoleScript().isAbleToInteractWithPlayers;
    }
}
