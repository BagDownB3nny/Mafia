using System.Linq;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{


    public void HostLobby()
    {
        // Start hosting a new lobby
        singleton.StartHost();
    }

    public void JoinLobby()
    {
        // Start joining a new lobby
        NetworkManager.singleton.StartClient();
    }

    public override void OnStopClient()
    {
        PlayerManager.instance.HandleNetworkStop();
        base.OnStopClient();
    }

    public override void OnStopHost()
    {
        PlayerManager.instance.HandleNetworkStop();
        base.OnStopHost();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == "Game")
        {
            OnGameSceneStarted();
        }
    }

    private void OnGameSceneStarted()
    {
        HouseSpawner.instance.InstantiateHouses();
    }

    public override void ServerChangeScene(string newSceneName)
    {
        Debug.Log("Server is initiating scene change to: " + newSceneName);
        base.ServerChangeScene(newSceneName);
    }


    public override void OnClientSceneChanged()
    {
        Debug.Log("Client finished loading scene");
        base.OnClientSceneChanged();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("Server spawning player for connection: " + conn.connectionId);
        base.OnServerAddPlayer(conn);
    }
}
