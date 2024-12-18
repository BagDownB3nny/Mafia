using Mirror;
using Mirror.Examples.Basic;
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

    private Vector3 spawnPoint;

    public void Awake()
    {
        if (isServer)
        {
            spawnPoint = transform.position;
        }
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.GetComponent<MoveCamera>().SetCameraPosition(this.transform);
        Camera.main.transform.GetComponent<PlayerCamera>().SetOrientation(this.transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
        PlayerManager.instance.localPlayer = this;
        Debug.Log("Local player set");

        localPlayerGun = Camera.main.transform.Find("Gun").gameObject;

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
        steamUsername = "Player " + Random.Range(0, 1000);
        CmdUpdateSteamUsername(steamUsername);
        playerUIPrefab.text = steamUsername;
    }

    [Server]
    public void SetRole(Roles newRole)
    {
        role = newRole;
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

    public void OnRoleChanged(Roles oldRole, Roles newRole)
    {
        if (isLocalPlayer)
        {
            PlayerUIManager.instance.SetRoleText(newRole);
        }
    }

    [Server]
    public void TeleportToSpawn()
    {
        Debug.Log($"Teleporting {netId} to spawn");
        transform.position = spawnPoint;
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
