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

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.GetComponent<MoveCamera>().SetCameraPosition(this.transform);
        Camera.main.transform.GetComponent<PlayerCamera>().SetOrientation(this.transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
        PlayerManager.localPlayer = this;
        Debug.Log("Local player set");

        if (SteamManager.Initialized)
        {
            steamUsername = SteamFriends.GetPersonaName();
            CmdUpdateSteamUsername(steamUsername);
        }
        else
        {
            steamUsername = "Player " + Random.Range(0, 1000);
            CmdUpdateSteamUsername(steamUsername);
        }
        playerUIPrefab.text = steamUsername;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayerManager.instance.AddPlayer(steamUsername, netId);
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
}
