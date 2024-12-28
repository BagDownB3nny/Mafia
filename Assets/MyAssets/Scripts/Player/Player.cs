using System;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;



public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnRoleChanged))]
    public Roles role;

    [SyncVar(hook = nameof(OnUsernameChanged))]
    public string steamUsername;
    [SerializeField] private TMP_Text playerUIPrefab;

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
            {Roles.Seer, gameObject.GetComponentInChildren<Seer>(includeInactive: true)},
            {Roles.Guardian, gameObject.GetComponentInChildren<Guardian>(includeInactive: true)},
            {Roles.Mafia, gameObject.GetComponentInChildren<Mafia>(includeInactive: true)}
        };
    }

    [Client]
    private void StartCamera()
    {
        Camera.main.transform.GetComponent<MoveCamera>().SetCameraPosition(this.transform);
        Camera.main.transform.GetComponent<PlayerCamera>().SetOrientation(this.transform);
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
    public void SetRole(Roles newRole)
    {
        role = newRole;
        EnableRoleScript(newRole);

        house.SpawnRoom(newRole);
    }

    private void EnableRoleScript(Roles newRole)
    {
        roleScripts[newRole].enabled = true;
    }

    private void DisableRoleScriptsExcept(Roles roleToKeep)
    {
        foreach (Roles role in Enum.GetValues(typeof(Roles)))
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
    public void OnRoleChanged(Roles oldRole, Roles newRole)
    {
        if (isLocalPlayer)
        {
            PlayerUIManager.instance.SetRoleText(newRole);
        }
        EnableRoleScript(newRole);
    }

    [Server]
    public void TeleportToSpawn()
    {
        TeleportPlayer(house.spawnPoint.position);
    }

    [Server]
    public void TeleportPlayer(Vector3 position)
    {
        RpcTeleportPlayer(position);
    }

    [ClientRpc]
    private void RpcTeleportPlayer(Vector3 position)
    {
        NetworkTransformBase networkTransform = GetComponent<NetworkTransformBase>();
        networkTransform.OnTeleport(position);
        Physics.SyncTransforms();
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
}
