using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class TargetDummyManager : NetworkBehaviour
{

    [SyncVar(hook = nameof(OnCursedTargetDummyChanged))]
    public TargetDummy cursedTargetDummy;
    
    [SerializeField] private GameObject targetDummiesParent;
    private List<TargetDummy> targetDummies;
    [SerializeField] private Whiteboard whiteboard;

    public static TargetDummyManager instance;

    private bool isShot = false;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            return;
        }
    }

    public void Start()
    {
        if (isServer)
        {
            PubSub.Subscribe<PlayerDeathEventHandler>(PubSubEvent.PlayerDeath, OnPlayerDeath);
            PubSub.Subscribe<HouseDestroyedEventHandler>(PubSubEvent.HouseDestroyed, OnHouseDestroyed);
        }
    }

    public List<TargetDummy> GetTargetDummies()
    {
        if (targetDummies == null)
        {
            targetDummies = targetDummiesParent.GetComponentsInChildren<TargetDummy>().ToList();
        }
        return targetDummies;
    }

    [Server]
    public void ClearSelection()
    {
        isShot = false;
        cursedTargetDummy = null;
    }

    [Server]
    private void OnPlayerDeath(Player player)
    {
        TargetDummy targetdummy = GetTargetdummyFromPlayer(player);
        targetdummy.isOccupantDead = true;
    }

    [Server]
    public void OnHouseDestroyed(House house)
    {
        TargetDummy targetdummy = GetTargetdummyFromHouse(house);
        targetdummy.isHouseDestroyed = true;

        if (cursedTargetDummy == targetdummy)
        {
            SetSelectedTargetdummy(null);
            isShot = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetSelectedTargetdummy(TargetDummy targetdummy, NetworkConnectionToClient connection)
    {
        if (isShot)
        {
            PlayerUIManager.instance.RpcSetTemporaryInteractableText(connection, "The mafia have already launched an attack tonight", 1.5f);
            return;
        }
        SetSelectedTargetdummy(targetdummy);
    }

    [Server]
    public void SetSelectedTargetdummy(TargetDummy targetdummy)
    {
        if (cursedTargetDummy != null)
        {
            cursedTargetDummy.UnmarkHouse();
        }
        cursedTargetDummy = targetdummy;
        cursedTargetDummy?.MarkHouse();
    }

    [Client]
    private void OnCursedTargetDummyChanged(TargetDummy oldTargetdummy, TargetDummy newTargetdummy)
    {
        // Set whiteboard to show selected house
        if (newTargetdummy == null)
        {
            whiteboard.ClearWhiteboard();
        }
        else
        {
            if (newTargetdummy.playerName == null)
            {
                whiteboard.SetNewMarkedPlayer("UNNAMED RAT");
                return;
            }
            else
            {
                whiteboard.SetNewMarkedPlayer(newTargetdummy.playerName);
            }
        }

        SetMafiaAttackInformativeText(newTargetdummy);
    }

    [Client]
    public void SetMafiaAttackInformativeText(TargetDummy targetdummy)
    {
        if (NetworkClient.localPlayer.GetComponent<Player>() != null)
        {
            Player localPlayer = NetworkClient.localPlayer.GetComponent<Player>();
            if (localPlayer.role == RoleName.Mafia && targetdummy != null)
            {
                PlayerUIManager.instance.SetInformativeText($"Attack {targetdummy.playerName}'s house!");
            }
        }
    }

    public TargetDummy GetTargetdummyFromNetId(uint netId)
    {
        if (NetworkServer.spawned.ContainsKey(netId))
        {
            return NetworkServer.spawned[netId].GetComponent<TargetDummy>();
        }
        else if (NetworkClient.spawned.ContainsKey(netId))
        {
            return NetworkClient.spawned[netId].GetComponent<TargetDummy>();
        }
        return null;
    }

    [Server]
    private TargetDummy GetTargetdummyFromPlayer(Player player)
    {
        foreach (TargetDummy targetDummy in GetTargetDummies())
        {
            if (targetDummy.linkedPlayer == player)
            {
                return targetDummy;
            }
        }
        return null;
    }

    [Server]
    private TargetDummy GetTargetdummyFromHouse(House house)
    {
        foreach (TargetDummy targetdummy in GetTargetDummies())
        {
            if (targetdummy && targetdummy.linkedHouse && targetdummy.linkedHouse.netId == house.netId)
            {
                return targetdummy;
            }
        }
        return null;
    }
}