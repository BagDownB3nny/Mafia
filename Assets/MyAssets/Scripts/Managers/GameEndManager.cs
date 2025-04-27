using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameEndManager : NetworkBehaviour
{
    private List<Player> alivePlayers = new List<Player>();
    private List<Player> aliveMafiaPlayers = new List<Player>();
    public static GameEndManager instance;

    private void Awake()
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

    public void Start()
    {
        PubSub.Subscribe<PlayerDeathEventHandler>(PubSubEvent.PlayerDeath, OnPlayerDeath);
        Debug.Log("GameEndManager started");
    }

    [Server]
    public void StartGame()
    {
        alivePlayers = PlayerManager.instance.GetAllPlayers();
        aliveMafiaPlayers = PlayerManager.instance.GetMafiaPlayers();
        Debug.Log($"Alive Players: {alivePlayers.Count}");
        Debug.Log($"Alive Mafia Players: {aliveMafiaPlayers.Count}");
    }

    [Server]
    public void OnPlayerDeath(Player player)
    {
        alivePlayers.Remove(player);
        if (player.role == RoleName.Mafia)
        {
            aliveMafiaPlayers.Remove(player);
        }
        if (isGameEnd())
        {
            // ShowGameEndScreen();
            // Invoke("EndGame", 3f); // 3 second delay
        }
    }

    private void EndGame()
    {
        PubSub.ClearAllSubscriptions();
        NetworkManager.singleton.ServerChangeScene("Lobby");
    }

    private bool isGameEnd()
    {
        return IsMafiaWin() || IsVillagerWin();
    }

    [Server]
    private void ShowGameEndScreen()
    {
        if (IsMafiaWin())
        {
            ShowMafiaWin();
        }
        else if (IsVillagerWin())
        {
            ShowVillagerWin();
        }
    }

    private bool IsMafiaWin()
    {
        return aliveMafiaPlayers.Count >= (alivePlayers.Count - aliveMafiaPlayers.Count);
    }

    private bool IsVillagerWin()
    {
        return aliveMafiaPlayers.Count == 0;
    }

    [Server]
    private void ShowMafiaWin()
    {
        RpcShowMafiaWin();
    }

    [ClientRpc]
    private void RpcShowMafiaWin()
    {
        GameEndScreen.instance.ShowMafiaWin();
    }

    [Server]
    private void ShowVillagerWin()
    {
        RpcShowVillagerWin();
    }

    [ClientRpc]
    private void RpcShowVillagerWin()
    {
        GameEndScreen.instance.ShowVillagerWin();
    }
}
