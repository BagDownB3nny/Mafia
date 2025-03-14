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
        singleton.StartClient();
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

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        // Assign the player to a house after they have been added to the server
        House playerHouse = startPos.GetComponentInParent<House>();
        Player playerComponent = player.GetComponent<Player>();
        if (playerHouse != null)
        {
            playerComponent.house = playerHouse;
            playerHouse.AssignPlayer(playerComponent);
        }
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
        HouseManager.instance.InstantiateHouses();
        MafiaHouseTable.instance.InstantiateHouseMinis();
    }

    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }


    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }
}
