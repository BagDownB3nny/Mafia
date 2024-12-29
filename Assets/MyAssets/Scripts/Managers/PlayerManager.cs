using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class SyncListPlayer : SyncList<NetworkIdentity> { };

public class PlayerManager : NetworkBehaviour
{
    // Is singleton
    public static PlayerManager instance;

    // A list containing all the roles for the current lobby
    // This list should be updated everytime a player joins or leaves the lobby
    // For example, in a 6 player game, there could be 1 werewolf, 1 seer, 1 medium, and 3 villagers
    public Roles[] playerRoles;

    public Player localPlayer;

    public readonly SyncDictionary<string, uint> playerNetIds = new SyncDictionary<string, uint>();

    [SyncVar]
    public int playerCount;


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
        playerNetIds[username] = playerNetId;
        playerCount = playerNetIds.Count;
        if (playerNetIds.Count == NetworkServer.connections.Count)
        {
            OnAllPlayersLoaded();
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
    public void AssignRoles()
    {
        playerRoles = GenerateRoles();
        // Shuffle the roles
        // playerRoles = playerRoles.OrderBy(x => Random.value).ToArray();

        int index = 0;
        foreach (Player player in GetAllPlayers())
        {
            if (player.name == "Player [connId=0]")
            {
                player.SetRole(Roles.Mafia);
            }
            else
            {
                player.SetRole(Roles.Seer);
            }
            // player.SetRole(playerRoles[index]);
            // index++;
        }
    }

    [Server]
    public Roles[] GenerateRoles()
    {
        // This is just a placeholder
        // In a real game, you would have a more complex algorithm to generate roles
        // For example, in a 6 player game, there could be 1 werewolf, 1 seer, 1 medium, and 3 villagers
        Roles[] roles = new Roles[playerNetIds.Count];
        for (int i = 0; i < roles.Length; i++)
        {
            if (i == 0)
            {
                roles[i] = Roles.SixthSense;
            }
            else if (i == 1)
            {
                roles[i] = Roles.Mafia;
            }
            else if (i == 2)
            {
                roles[i] = Roles.Guardian;
            }
            else
            {
                roles[i] = Roles.Villager;
            }
        }
        return roles;
    }

    public List<Player> GetAllPlayers()
    {
        List<Player> players = new List<Player>();
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
        return NetworkServer.spawned[playerNetIds[name]].GetComponent<Player>();
    }

    [Server]
    public void TeleportAllPlayersBackToSpawn()
    {
        foreach (Player player in GetAllPlayers())
        {
            player.TeleportToSpawn();
        }
    }

    [Server]
    public void TeleportAllPlayersBackToNightSpawn()
    {
        foreach (Player player in GetAllPlayers())
        {
            player.TeleportToNightSpawn();
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
            if (player.role == Roles.Mafia)
            {
                mafiaPlayers.Add(player);
            }
        }
        return mafiaPlayers;
    }
}
