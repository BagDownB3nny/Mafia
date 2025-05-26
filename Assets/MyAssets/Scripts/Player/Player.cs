using Steamworks;
using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnRoleChanged))]
    public RoleName role;
    [SerializeField] private TMP_Text playerUIPrefab;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject playerVisual;

    [Header("Name")]
    [SyncVar(hook = nameof(OnUsernameChanged))]

    public string steamUsername;
    [SerializeField] private Billboard playerNameTag;


    [SyncVar]
    public House house;

    [Header("Role scripts")]

    [SerializeField] private GameObject mafiaScripts;
    [SerializeField] private GameObject seerScripts;
    [SerializeField] private GameObject guardianScripts;
    [SerializeField] private GameObject sixthSenseScripts;
    [SerializeField] private GameObject villagerScripts;
    [SerializeField] private GameObject mediumScripts;

    Dictionary<Enum, GameObject> roleScripts;

    [Header("Guardian params")]
    public bool isProtected = false;

    public void Start()
    {
        GetRoleScripts();
    }

    public override void OnStartLocalPlayer()
    {
        StartCamera();
        GetSteamUsername();
        SetLayer();

        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.localPlayer = this;
        }
    }

    [Client]
    private void SetLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    [Client]
    private void GetRoleScripts()
    {
        roleScripts = new Dictionary<Enum, GameObject>
        {
            { RoleName.Villager, villagerScripts },
            { RoleName.SixthSense, sixthSenseScripts },
            { RoleName.Guardian, guardianScripts },
            { RoleName.Seer, seerScripts },
            { RoleName.Mafia, mafiaScripts },
            { RoleName.Medium, mediumScripts}
        };
    }

    [Server]
    public void RemoveNametag()
    {
        RpcRemoveNametag();
    }

    [ClientRpc]
    public void RpcRemoveNametag()
    {
        playerNameTag.gameObject.SetActive(false);
    }

    [Server]
    public void AddNametag()
    {
        RpcAddNametag();
    }

    [ClientRpc]
    public void RpcAddNametag()
    {
        playerNameTag.gameObject.SetActive(true);
    }

    [Client]
    private void StartCamera()
    {
        MoveCamera moveCamera = Camera.main.transform.GetComponent<MoveCamera>();
        moveCamera.SetCameraPosition(cameraTransform);
        moveCamera.playerDefaultCameraPosition = cameraTransform;

        PlayerCamera.instance.SetOrientation(this.transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
    }

    [Client]
    public void GetSteamUsername()
    {
        string username;
        // if (SteamManager.Initialized)
        // {
        //     Debug.Log("Getting Steam username for player");
        //     username = SteamFriends.GetPersonaName();
        //     CmdUpdateSteamUsername(username);
        // }
        // else
        // {
        username = "Player " + UnityEngine.Random.Range(0, 1000);
        CmdUpdateSteamUsername(username);
        // }
    }

    [Server]
    public void GiveSpectatorMode()
    {
        GetComponent<PlayerTeleporter>().TeleportToSpectatorSpawn();
        GetComponent<PlayerDeath>().isDead = true;
        GetComponent<PlayerMovement>().OnDeath(this);
        // Did this instead of kill because we don't want unintended side effects 
        // Eg. Death pubsub event, corpse spawning, etc.
    }

    [Command]
    private void CmdUpdateSteamUsername(string newUsername)
    {
        steamUsername = newUsername;
        Debug.Log($"Updated steam username: {steamUsername}");
        if (PlayerManager.instance)
        {
            NetworkConnectionToClient conn = connectionToClient;
            int connectionId = conn.connectionId;
            PlayerManager.instance.AddPlayer(this);
        }
    }

    public Role GetRoleScript()
    {
        if (!roleScripts.ContainsKey(role)) return null;
        GameObject roleObject = roleScripts[role];
        Role roleScript = roleObject.GetComponentInChildren<Role>(includeInactive: true);
        return roleScript;
    }

    public RoleActions GetRoleActions()
    {
        if (!roleScripts.ContainsKey(role)) return null;
        return roleScripts[role].GetComponent<RoleActions>();
    }

    [Server]
    public void SetRole(RoleName newRole)
    {
        role = newRole;
        EnableRoleScript(newRole);
        DisableRoleScriptsExcept(newRole);
        house.SpawnRoom(newRole);
    }

    [Client]
    public RoleName GetRole()
    {
        return role;
    }

    private void EnableRoleScript(RoleName newRole)
    {
        roleScripts[newRole].SetActive(true);
    }

    private void DisableRoleScriptsExcept(RoleName roleToKeep)
    {
        foreach (RoleName role in Enum.GetValues(typeof(RoleName)))
        {
            if (role == roleToKeep)
            {
                roleScripts[role].SetActive(true);
            }
            else
            {
                if (roleScripts.ContainsKey(role))
                {
                    roleScripts[role].SetActive(false);
                }
            }
        }
    }

    [Client]
    private void OnUsernameChanged(string oldUsername, string newUsername)
    {
        playerUIPrefab.text = newUsername;
        if (house)
        {
            Debug.Log($"Nameplate updated house -> player{newUsername}");
            house.SetNameplateText(newUsername);
        }
    }

    [Client]
    public void OnRoleChanged(RoleName oldRole, RoleName newRole)
    {
        if (isLocalPlayer)
        {
            PlayerUIManager.instance.SetRoleText(newRole);
            PlayerUIManager.instance.SetRolePromptText(newRole);
            EnableRoleScript(newRole);
            DisableRoleScriptsExcept(newRole);
            PlayerUIManager.instance.SetAllPlayerTexts();
        }
        else if (newRole == RoleName.Mafia && PlayerManager.instance.localPlayer.role == RoleName.Mafia)
        {
            SetNameTagColor(Color.red);
        }

        if (isLocalPlayer && newRole != RoleName.Mafia)
        {
            PlayerUIManager.instance.SetVillagerTexts();
        }
        else if (isLocalPlayer && newRole == RoleName.Mafia)
        {
            house.EnableTrapdoor();
            PlayerUIManager.instance.SetMafiaTexts();
        }

        if (newRole == RoleName.Medium)
        {
            house.SetDoorToGhostLayer();
        }
    }

    [Client]
    public void SetNameTagColor(Color color)
    {
        playerUIPrefab.color = color;
    }

    [Server]
    public void SetAbleToSeeNametags(bool ableToSee)
    {
        RpcSetAbleToSeeNametags(connectionToClient, ableToSee);
    }

    [TargetRpc]
    public void RpcSetAbleToSeeNametags(NetworkConnectionToClient target, bool ableToSee)
    {
        if (ableToSee)
        {
            CameraCullingMaskManager.instance.SetNameTagLayerVisible();
        }
        else
        {
            CameraCullingMaskManager.instance.SetNameTagLayerInvisible();
        }
    }

    [Client]
    public void DisablePlayerControllersAndCamera()
    {
        GetComponent<PlayerMovement>().enabled = false;
        Camera.main.GetComponent<PlayerCamera>().EnterSpectatorMode();
    }

    [Client]
    public void EnablePlayerControllersAndCamera()
    {
        GetComponent<PlayerMovement>().enabled = true;
        Camera.main.GetComponent<MoveCamera>().enabled = true;
        Camera.main.GetComponent<PlayerCamera>().EnterFPSMode();
    }
}
