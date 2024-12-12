using Mirror;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
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
