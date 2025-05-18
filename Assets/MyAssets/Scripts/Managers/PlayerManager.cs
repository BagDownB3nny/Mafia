using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic;

public class PlayerManager : NetworkBehaviour
{
    // Is singleton
    public static PlayerManager instance;

    // Is false if the scene is lobby, is true if the scene is game
    [SerializeField] public bool isGamePlayerManager;

    // A list containing all the roles for the current lobby
    // This list should be updated everytime a player joins or leaves the lobby
    // For example, in a 6 player game, there could be 1 werewolf, 1 seer, 1 medium, and 3 villagers
    public RoleName[] playerRoles;

    public Player localPlayer;

    // Key: username, Value: playerNetId
    public readonly SyncDictionary<string, uint> playerNetIds = new SyncDictionary<string, uint>();

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
    public void AddPlayer(string username, uint playerNetId)
    {
        if (!isGamePlayerManager) return;

        playerNetIds[username] = playerNetId;
        playerCount = playerNetIds.Count;
        if (playerNetIds.Count == NetworkServer.connections.Count && !isGameStarted)
        {
            OnAllPlayersLoaded();
            isGameStarted = true;
        }
        else if (isGameStarted)
        {
            Player joinedPlayer = GetPlayerByNetId(playerNetId);
            joinedPlayer.GiveSpectatorMode();
        }
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
            if (player.name == "Player [connId=0]")
            {
                player.SetRole(RoleName.Guardian);
            }
            else
            {
                player.SetRole(RoleName.Mafia);
            }
            // player.SetRole(playerRoles[index]);
            // index++;
        }
    }

    [Server]
    public RoleName[] GenerateRoles()
    {
        // This is just a placeholder
        // In a real game, you would have a more complex algorithm to generate roles
        // For example, in a 6 player game, there could be 1 werewolf, 1 seer, 1 medium, and 3 villagers
        RoleName[] roles = new RoleName[playerNetIds.Count];

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
        foreach (uint netId in playerNetIds.Values)
        {
            players.Add(GetPlayerByNetId(netId));
        }
        return players;
    }

    public List<string> GetAllPlayerNames()
    {
        return playerNetIds.Keys.ToList();
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

    public Player GetPlayerByName(string name)
    {
        if (NetworkServer.spawned.ContainsKey(playerNetIds[name]))
        {
            return NetworkServer.spawned[playerNetIds[name]].GetComponent<Player>();
        }
        else if (NetworkClient.spawned.ContainsKey(playerNetIds[name]))
        {
            return NetworkClient.spawned[playerNetIds[name]].GetComponent<Player>();
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

    [Server]
    public void EnableMediumInteractions()
    {
        foreach (Player player in GetMediumPlayers())
        {
            Medium role = player.GetRoleScript() as Medium;
            role.EnableMediumAbility();
        }
        HouseManager.instance.HighlightMediumHouseForGhosts();
    }

    [Server]
    public void DisableMediumInteractions()
    {
        foreach (Player player in GetMediumPlayers())
        {
            Medium role = player.GetRoleScript() as Medium;
            role.DisableMediumAbility();
        }
        HouseManager.instance.UnhighlightMediumHouseForGhosts();
    }
}
