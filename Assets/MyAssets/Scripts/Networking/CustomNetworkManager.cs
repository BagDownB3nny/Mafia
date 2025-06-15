using System.Collections;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{

    public override void Awake()
    {
        base.Awake();
        FileLogger.Log("Starting game and filelogger");
    }

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
        base.OnStopClient();
    }

    public override void OnStopHost()
    {
        PlayerManager.instance.HandleNetworkStop();
        base.OnStopHost();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (PlayerManager.instance && PlayerManager.instance.isGameStarted)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // Instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        // Assign the player to a house after they have been added to the server
        House playerHouse = startPos.GetComponentInParent<House>();
        Player playerComponent = player.GetComponent<Player>();
        if (playerHouse != null)
        {
            AssignPlayerToHouse(playerHouse, playerComponent);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // if (SteamManager.Initialized)
        // {
        //     SteamMatchmaking.LeaveLobby(currentLobbyID); // Leave Steam lobby
        // }
        
        if (PlayerManager.instance && !PlayerManager.instance.ConnIdToNetIdDict.Keys.Contains(conn.connectionId))
        {
            // If player is trying to join game mid-game, playermanager will not contain their connId
            base.OnServerDisconnect(conn);
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Game")
        {
            OnDisconnectFromGame(conn);
        } else if (currentScene == "Lobby")
        {
            OnDisconnectFromLobby(conn);
        }
        else
        {
            Debug.LogError($"Client disconnected, scene not handled: {currentScene}");
        }
        // Base method destroys connection's player object
        base.OnServerDisconnect(conn);
    }

    [Server]
    public void OnDisconnectFromGame(NetworkConnectionToClient conn)
    {
        int connectionId = conn.connectionId;
        Player player = PlayerManager.instance.GetPlayerByConnId(connectionId);
        player.GetComponent<PlayerDeath>().ServerKillPlayer(false);
        PlayerManager.instance.RemovePlayer(connectionId);
    }

    [Server]    
    public void OnDisconnectFromLobby(NetworkConnectionToClient conn)
    {
        int connectionId = conn.connectionId;
        PlayerManager.instance.RemovePlayer(connectionId);
        // Player manager removal must be first to be called
        
        PlayerColourManager.instance.OnPlayerLeftLobby(conn);
        RoleSettingsMenu.instance.OnPlayerLeftLobby(conn);
    }

    public void AssignPlayerToHouse(House playerHouse, Player player)
    {
        player.house = playerHouse;
        playerHouse.AssignPlayer(player);
    }

    // public override void OnServerSceneChanged(string sceneName)
    // {
    //     base.OnServerSceneChanged(sceneName);

    //     if (sceneName == "Game")
    //     {
    //         OnGameSceneStarted();
    //     }
    // }

    // private void OnGameSceneStarted()
    // {
    //     HouseManager.instance.InstantiateHouses();
    //     MafiaHouseTable.instance.InstantiateHouseMinis();
    // }

    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }


    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }
}
