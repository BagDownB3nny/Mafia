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

        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.localPlayer = this;
        }

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
        //     steamUsername = "Player " + UnityEngine.Random.Range(0, 1000);
        //     CmdUpdateSteamUsername(steamUsername);
        // }
        steamUsername = "Player " + UnityEngine.Random.Range(0, 1000);
        CmdUpdateSteamUsername(steamUsername);
        playerUIPrefab.text = steamUsername;
    }

    [Command]
    private void CmdUpdateSteamUsername(string newUsername)
    {
        steamUsername = newUsername;
        Debug.Log($"Updated steam username: {steamUsername}");
        if (PlayerManager.instance)
        {
            PlayerManager.instance.AddPlayer(steamUsername, netId);
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
        Debug.Log($"{gameObject.name} updated username text: {newUsername}");
        if (house)
        {
            house.namePlateText.text = newUsername;
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
}
