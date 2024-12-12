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

    public static Player localPlayer;

    [SyncVar(hook = nameof(OnPlayerNetIdsChanged))]
    public readonly SyncDictionary<string, uint> playerNetIds = new SyncDictionary<string, uint>();

    public Dictionary<string, uint> playerNetIdsView = new Dictionary<string, uint>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        playerNetIdsView = playerNetIds.ToDictionary(x => x.Key, x => x.Value);
        if (playerNetIds.Count == NetworkServer.connections.Count)
        {
            OnAllPlayersLoaded();
        }
    }

    private void OnPlayerNetIdsChanged(SyncDictionary<string, uint>.Operation op, string key, uint value)
    {
        playerNetIdsView = playerNetIds.ToDictionary(x => x.Key, x => x.Value);
        Debug.Log("Client updated");
    }

    public void HandleNetworkStop()
    {
        // Destroy(gameObject);
    }

    [Server]
    public void OnAllPlayersLoaded()
    {
        AssignRoles();
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
        playerRoles = playerRoles.OrderBy(x => Random.value).ToArray();

        int index = 0;
        foreach (Player player in GetAllPlayers())
        {
            player.SetRole(playerRoles[index]);
            index++;
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
                roles[i] = Roles.WereRat;
            }
            else if (i == 1)
            {
                roles[i] = Roles.Seer;
            }
            else if (i == 2)
            {
                roles[i] = Roles.Medium;
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
        return playerNetIds.Select(x => NetworkServer.spawned[x.Value].GetComponent<Player>()).ToList();
    }
}
