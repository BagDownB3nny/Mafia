using Mirror;
using UnityEngine;

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
        if (NetworkServer.active)
        {
            // [ASSUMPTION] The host is acting as the server, there is no dedicated server.
            NetworkManager.singleton.StopHost();
            RoleManager.roleDict.Clear();
            RoleSettingsMenu.expectedPlayerCountIncreased = false;
            RoleSettingsMenu.roleSettingsChanged = false;
        }
        if (NetworkClient.active)
        {
            NetworkClient.Disconnect();
        }
    }
}
