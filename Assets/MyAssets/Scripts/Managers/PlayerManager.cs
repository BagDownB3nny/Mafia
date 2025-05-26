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
    [SerializeField] public sceneName scene;

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
    public void AddLobbyPlayer(Player player, int connectionId)
    {
        PlayerColourManager.instance.OnPlayerJoinedLobby(player, connectionId);
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

    [Server]
    public void AssignRoles()
    {
        playerRoles = GenerateRoles();
        // Shuffle the roles
        playerRoles = playerRoles.OrderBy(x => Random.value).ToArray();

        int index = 0;
        foreach (Player player in GetAllPlayers())
        {
            // if (player.name == "Player [connId=0]")
            // {
            //     player.SetRole(RoleName.Medium);
            // }
            // else
            // {
            //     player.SetRole(RoleName.Medium);
            // }
            player.SetRole(playerRoles[index]);
            index++;
        }
    }

    [Server]
    public RoleName[] GenerateRoles()
    {
        // This is just a placeholder
        // In a real game, you would have a more complex algorithm to generate roles
        // For example, in a 6 player game, there could be 1 werewolf, 1 seer, 1 medium, and 3 villagers
        RoleName[] roles = new RoleName[ConnIdToUsernameDict.Count];

        for (int i = 0; i < roles.Length; i++)
        {
            if (i == 0 || i == 7 || i == 10)
            {
                roles[i] = RoleName.Mafia;
            }
            else if (i == 1 || i == 9)
            {
                roles[i] = RoleName.Seer;
            }
            else if (i == 2)
            {
                roles[i] = RoleName.Guardian;
            }
            else if (i == 3)
            {
                roles[i] = RoleName.Medium;
            }
            else if (i == 4)
            {
                roles[i] = RoleName.SixthSense;
            }
            else if (i == 5 || i == 6 || i == 8)
            {
                roles[i] = RoleName.Villager;
            }
        }
        return roles;
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
