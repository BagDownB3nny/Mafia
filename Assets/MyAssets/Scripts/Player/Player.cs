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
    [SerializeField] private GameObject playerVisual;

    [SyncVar]
    public House house;

    PlayerActions actions;
    Dictionary<Enum, Role> roleScripts;
    public bool isDead = false;

    [Header("Guardian params")]
    public bool isProtected = false;

    public void Start()
    {
        GetRoleScripts();
        this.actions = GetComponent<PlayerActions>();
        if (actions == null)
        {
            Debug.LogError("PlayerActions component not found on player object.");
        }
    }

    public override void OnStartLocalPlayer()
    {
        StartCamera();
        GetSteamUsername();

        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.localPlayer = this;
        }

        playerVisual.SetActive(false);
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
            PlayerUIManager.instance.SetRolePromptText(newRole);
            EnableRoleScript(newRole);
            this.actions.OnRoleChanged(newRole);
        }
    }

    public void SetNameTagColor(Color color)
    {
        playerUIPrefab.color = color;
    }

    public bool IsAbleToShoot()
    {
        Role roleScript = GetRoleScript();
        if (roleScript is Mafia mafiaRole)
        {
            return mafiaRole.HasGun();
        }
        return false;
    }

    public bool IsAbleToInteractWithPlayers()
    {
        if (GetRoleScript() == null)
        {
            return false;
        }
        return GetRoleScript().IsAbleToInteractWithPlayers;
    }

    public bool IsAbleToInteractWithDoors()
    {
        if (GetRoleScript() == null)
        {
            return false;
        }
        return GetRoleScript().IsAbleToInteractWithDoors;
    }
}
