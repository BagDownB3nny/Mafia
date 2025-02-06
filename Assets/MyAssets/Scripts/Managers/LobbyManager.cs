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
            NetworkServer.DisconnectAll();
        }
        else
        {
            NetworkClient.Disconnect();
        }
    }
}
