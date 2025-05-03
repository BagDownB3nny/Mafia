using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MafiaHouseTable : NetworkBehaviour
{

    [Header("House mini ref")]
    [SerializeField] private GameObject houseMiniPrefab;
    [SyncVar(hook = nameof(OnSelectedHouseMiniChanged))]
    public InteractableVillageHouseMini selectedHouseMini;

    public SyncList<uint> houseMinis = new SyncList<uint>();


    [SerializeField] private Whiteboard whiteboard;

    public static MafiaHouseTable instance;

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

    [Server]
    public void ClearSelection()
    {
        isShot = false;
        selectedHouseMini = null;
    }

    [Server]
    private void OnPlayerDeath(Player player)
    {
        InteractableVillageHouseMini houseMini = GetHouseMiniFromPlayer(player);
        houseMini.isOccupantDead = true;
    }

    [Server]
    public void OnHouseDestroyed(House house)
    {
        InteractableVillageHouseMini houseMini = GetHouseMiniFromHouse(house);
        houseMini.isHouseDestroyed = true;

        if (selectedHouseMini == houseMini)
        {
            SetSelectedHouseMini(null);
            isShot = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetSelectedHouseMini(InteractableVillageHouseMini houseMini, NetworkConnectionToClient connection)
    {
        if (isShot)
        {
            PlayerUIManager.instance.RpcSetTemporaryInteractableText(connection, "The mafia have already launched an attack tonight", 1.5f);
            return;
        }
        SetSelectedHouseMini(houseMini);
    }

    [Server]
    public void SetSelectedHouseMini(InteractableVillageHouseMini houseMini)
    {
        Debug.Log(houseMini);
        if (selectedHouseMini != null)
        {
            selectedHouseMini.UnmarkHouse();
        }

        selectedHouseMini = houseMini;
        selectedHouseMini?.MarkHouse();
    }


    [Server]
    public void InstantiateHouseMinis()
    {
        List<House> houses = HouseManager.instance.houses;

        foreach (House house in houses)
        {
            Vector3 housePosition = house.positionRelativeToVillageCenter;
            Vector3 houseMiniPositionRelativeToTableCenter = housePosition * 0.03f;

            Vector3 lookAtDirection = Vector3.zero - housePosition;
            Quaternion houseMiniRotation = Quaternion.LookRotation(lookAtDirection);


            GameObject houseMini = Instantiate(
                houseMiniPrefab,
                transform.position + houseMiniPositionRelativeToTableCenter,
                houseMiniRotation
            );

            NetworkServer.Spawn(houseMini);
            houseMini.GetComponent<InteractableVillageHouseMini>().LinkHouse(house);
            houseMinis.Add(houseMini.GetComponent<InteractableVillageHouseMini>().netId);
        }
    }

    [Client]
    private void OnSelectedHouseMiniChanged(InteractableVillageHouseMini oldHouseMini, InteractableVillageHouseMini newHouseMini)
    {
        if (oldHouseMini != null)
        {
            // Unmark the old house
            oldHouseMini.UnmarkHouse();
        }
        // Set whiteboard to show selected house
        if (newHouseMini == null)
        {
            whiteboard.ClearWhiteboard();
        }
        else
        {
            newHouseMini.MarkHouse();
            if (newHouseMini.PlayerName == null)
            {
                whiteboard.SetNewMarkedPlayer("UNNAMED RAT");
                return;
            }
            else
            {
                whiteboard.SetNewMarkedPlayer(newHouseMini.PlayerName);
            }
        }

        if (PlayerManager.instance.localPlayer != null)
        {
            Player localPlayer = PlayerManager.instance.localPlayer;
            if (localPlayer.role == RoleName.Mafia && newHouseMini != null)
            {
                PlayerUIManager.instance.SetInformativeText($"Attack {newHouseMini.PlayerName}'s house!");
            }
        }
    }

    public InteractableVillageHouseMini GetHouseMiniFromNetId(uint netId)
    {
        if (NetworkServer.spawned.ContainsKey(netId))
        {
            return NetworkServer.spawned[netId].GetComponent<InteractableVillageHouseMini>();
        }
        else if (NetworkClient.spawned.ContainsKey(netId))
        {
            return NetworkClient.spawned[netId].GetComponent<InteractableVillageHouseMini>();
        }
        return null;
    }

    [Server]
    private InteractableVillageHouseMini GetHouseMiniFromPlayer(Player player)
    {
        foreach (uint houseMiniNetId in houseMinis)
        {
            InteractableVillageHouseMini houseMini = GetHouseMiniFromNetId(houseMiniNetId);
            if (houseMini && houseMini.house && houseMini.house.player && houseMini.house.player.netId == player.netId)
            {
                return houseMini;
            }
        }
        return null;
    }

    [Server]
    private InteractableVillageHouseMini GetHouseMiniFromHouse(House house)
    {
        foreach (uint houseMiniNetId in houseMinis)
        {
            InteractableVillageHouseMini houseMini = GetHouseMiniFromNetId(houseMiniNetId);
            if (houseMini && houseMini.house && houseMini.house.netId == house.netId)
            {
                return houseMini;
            }
        }
        return null;
    }
}