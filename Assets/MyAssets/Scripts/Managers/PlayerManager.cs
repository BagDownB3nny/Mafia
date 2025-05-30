using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic;

public enum sceneName
{
    Lobby,
    Game
}

public class PlayerManager : NetworkBehaviour
{
    // Is singleton
    public static PlayerManager instance;

    // Is false if the scene is lobby, is true if the scene is game
    public sceneName scene;

    // A list containing all the roles for the current lobby
    // This list should be updated everytime a player joins or leaves the lobby
    // For example, in a 6 player game, there could be 1 werewolf, 1 seer, 1 medium, and 3 villagers
    public RoleName[] playerRoles;

    public Player localPlayer;

    // Key: username, Value: playerNetId
    public readonly SyncDictionary<int, string> ConnIdToUsernameDict = new SyncDictionary<int, string>();
    public readonly SyncDictionary<int, uint> ConnIdToNetIdDict = new SyncDictionary<int, uint>();

    [SyncVar]
    public int playerCount;

    public bool isGameStarted = false;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("More than one instance of PlayerManager found!");
            Destroy(gameObject);
            return;
        }
    }

    [Server]
    public void AddPlayer(Player player)
    {
        string username = player.steamUsername;
        int connectionId = player.connectionToClient.connectionId;

        ConnIdToUsernameDict[connectionId] = username;
        ConnIdToNetIdDict[connectionId] = player.netId;

        if (scene == sceneName.Lobby)
        {
            AddLobbyPlayer(player, connectionId);
        }
        else if (scene == sceneName.Game)
        {
            AddGamePlayer(connectionId);
        }
    }

    [Server]
    public void RemovePlayer(int connectionId)
    {
        ConnIdToUsernameDict.Remove(connectionId);
        ConnIdToNetIdDict.Remove(connectionId);
    }

    [Server]
    public bool CanStartGame()
    {
        int expectedPlayerCount = RoleSettingsMenu.instance.expectedPlayerCount;
        int playerCount = GetPlayerCount();
        int roleCount = RoleSettingsMenu.instance.GetRoleCount();
        if (playerCount == expectedPlayerCount && roleCount == expectedPlayerCount)
        {
            return true;
        }
        return false;
    }

    public int GetPlayerCount()
    {
        return ConnIdToUsernameDict.Count;
    }

    [Server]
    public void AddLobbyPlayer(Player player, int connectionId)
    {
        PlayerColourManager.instance.OnPlayerJoinedLobby(player, connectionId);
        RoleSettingsMenu.instance.OnPlayerJoin();
    }

    public int GetConnIdByNetId(uint netId)
    {
        foreach (KeyValuePair<int, uint> kvp in ConnIdToNetIdDict)
        {
            if (kvp.Value == netId)
            {
                return kvp.Key;
            }
        }
        return -1;
    }

    [Client]
    public int LocalPlayerConnId()
    {
        return GetConnIdByNetId(localPlayer.netId);
    }

    [Server]
    public void AddGamePlayer(int connectionId)
    {
        PlayerColourManager.instance.OnPlayerJoinedGame(GetPlayerByConnId(connectionId), connectionId);
        if (ConnIdToUsernameDict.Count == NetworkServer.connections.Count && !isGameStarted)
        {
            OnAllPlayersLoaded();
            isGameStarted = true;
        }
        else if (isGameStarted)
        {
            Player joinedPlayer = GetPlayerByConnId(connectionId);
            joinedPlayer.GiveSpectatorMode();
        }
    }

    public Player GetPlayerByConnId(int connId)
    {
        uint netId = ConnIdToNetIdDict[connId];
        return GetPlayerByNetId(netId);
    }

    public void HandleNetworkStop()
    {
        // Destroy(gameObject);
    }

    [Server]
    public void OnAllPlayersLoaded()
    {
        GameManager.instance.StartGame();
    }

    [Client]
    public string GetLocalPlayerName()
    {
        return localPlayer.steamUsername;
    }

    [Server]
    public void RemoveAllNametagsForNonMafia()
    {
        foreach (Player player in GetNonMafiaPlayers())
        {
            player.SetAbleToSeeNametags(false);
        }
    }

    [Server]
    public void AddAllNametagsForNonMafia()
    {
        foreach (Player player in GetNonMafiaPlayers())
        {
            player.SetAbleToSeeNametags(true);
        }
    }
    public List<Player> GetAllPlayers()
    {
        List<Player> players = new();
        foreach (uint netId in ConnIdToNetIdDict.Values)
        {
            players.Add(GetPlayerByNetId(netId));
        }
        return players;
    }


    public List<string> GetAllPlayerNames()
    {
        return ConnIdToUsernameDict.Values.ToList();
    }

    public List<int> GetAllPlayerConnIds()
    {
        return ConnIdToUsernameDict.Keys.ToList();
    }

    public string GetPlayerUsernameByConnId(int connId)
    {
        if (ConnIdToUsernameDict.ContainsKey(connId))
        {
            return ConnIdToUsernameDict[connId];
        }
        else
        {
            return null;
        }
    }


    public Player GetPlayerByNetId(uint netId)
    {
        if (NetworkServer.spawned.ContainsKey(netId))
        {
            return NetworkServer.spawned[netId].GetComponent<Player>();
        }
        else if (NetworkClient.spawned.ContainsKey(netId))
        {
            return NetworkClient.spawned[netId].GetComponent<Player>();
        }
        else
        {
            return null;
        }
    }

    [Server]
    public void TeleportAllPlayersBackToSpawn()
    {
        foreach (Player player in GetAllPlayers())
        {
            player.GetComponent<PlayerTeleporter>().TeleportToSpawn();
        }
    }

    [Server]
    public void TeleportAllPlayersBackToNightSpawn()
    {
        foreach (Player player in GetAllPlayers())
        {
            player.GetComponent<PlayerTeleporter>().TeleportToNightSpawn();
        }
    }

    public int GetMafiaCount()
    {
        return GetMafiaPlayers().Count;
    }

    public List<Player> GetMafiaPlayers()
    {
        List<Player> mafiaPlayers = new List<Player>();
        foreach (Player player in GetAllPlayers())
        {
            if (player.role == RoleName.Mafia)
            {
                mafiaPlayers.Add(player);
            }
        }
        return mafiaPlayers;
    }

    public List<Player> GetNonMafiaPlayers()
    {
        List<Player> nonMafiaPlayers = new();
        foreach (Player player in GetAllPlayers())
        {
            if (player.role != RoleName.Mafia)
            {
                nonMafiaPlayers.Add(player);
            }
        }
        return nonMafiaPlayers;
    }

    public List<Player> GetDeadPlayers()
    {
        List<Player> deadPlayers = new();
        foreach (Player player in GetAllPlayers())
        {
            bool isDead = player.GetComponent<PlayerDeath>().isDead;
            if (isDead)
            {
                deadPlayers.Add(player);
            }
        }
        return deadPlayers;
    }

    public List<Player> GetMediumPlayers()
    {
        List<Player> mediumPlayers = new();
        foreach (Player player in GetAllPlayers())
        {
            if (player.role == RoleName.Medium)
            {
                mediumPlayers.Add(player);
            }
        }
        return mediumPlayers;
    }
}
