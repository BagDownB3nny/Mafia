using Mirror;
using UnityEngine;
using Steamworks;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    public void StartGame()
    {
        NetworkManager.singleton.ServerChangeScene("Game");
    }

    public void EndGame()
    {
        CSteamID lobbyCode = new CSteamID(ulong.Parse(SteamLobby.instance.LobbyCode));
        if (NetworkServer.active)
        {
            // [ASSUMPTION] The host is acting as the server, there is no dedicated server.
            NetworkManager.singleton.StopHost();
            // SteamMatchmaking.LeaveLobby(lobbyCode);
            RoleManager.roleDict.Clear();
            RoleSettingsMenu.expectedPlayerCountIncreased = false;
            RoleSettingsMenu.roleSettingsChanged = false;
        }
        if (NetworkClient.active)
        {
            NetworkClient.Disconnect();
            // SteamMatchmaking.LeaveLobby(lobbyCode);
        }
    }
}
