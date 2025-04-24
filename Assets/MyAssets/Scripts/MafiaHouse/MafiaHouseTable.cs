using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MafiaHouseTable : NetworkBehaviour
{
    [SerializeField] private GameObject houseMiniPrefab;
    [SerializeField] private Transform houseMiniParent;
    [SerializeField] private Whiteboard whiteboard;

    public static MafiaHouseTable instance;

    [SyncVar(hook = nameof(OnSelectedHouseMiniChanged))]
    public InteractableVillageHouseMini selectedHouseMini;

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
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetSelectedHouseMini(InteractableVillageHouseMini houseMini)
    {
        SetSelectedHouseMini(houseMini);
    }

    [Server]
    public void SetSelectedHouseMini(InteractableVillageHouseMini houseMini)
    {
        if (selectedHouseMini)
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
            Vector3 houseMiniPositionRelativeToTableCenter = housePosition * 0.02f;

            Vector3 lookAtDirection = Vector3.zero - housePosition;
            Quaternion houseMiniRotation = Quaternion.LookRotation(lookAtDirection);


            GameObject houseMini = Instantiate(
                houseMiniPrefab,
                transform.position + houseMiniPositionRelativeToTableCenter,
                houseMiniRotation
            );

            NetworkServer.Spawn(houseMini);
            houseMini.GetComponent<InteractableVillageHouseMini>().LinkHouse(house);
        }
    }

    [Client]
    private void OnSelectedHouseMiniChanged(InteractableVillageHouseMini oldHouseMini, InteractableVillageHouseMini newHouseMini)
    {
        Debug.Log($"Selected house mini changed: {oldHouseMini} -> {newHouseMini}");
        if (newHouseMini == null)
        {
            whiteboard.ClearWhiteboard();
        }
        else
        {
            if (newHouseMini.playerName == null)
            {
                whiteboard.SetNewMarkedPlayer("UNNAMED RAT");
                return;
            }
            else
            {
                whiteboard.SetNewMarkedPlayer(newHouseMini.playerName);
            }
        }
    }

    [Server]
    private InteractableVillageHouseMini GetHouseMiniFromPlayer(Player player)
    {
        foreach (InteractableVillageHouseMini houseMini in houseMiniParent.GetComponentsInChildren<InteractableVillageHouseMini>())
        {
            if (houseMini.house && houseMini.house.player && houseMini.house.player.netId == player.netId)
            {
                return houseMini;
            }
        }
        return null;
    }

    [Server]
    private InteractableVillageHouseMini GetHouseMiniFromHouse(House house)
    {
        foreach (InteractableVillageHouseMini houseMini in houseMiniParent.GetComponentsInChildren<InteractableVillageHouseMini>())
        {
            if (houseMini.house && houseMini.house.netId == house.netId)
            {
                return houseMini;
            }
        }
        return null;
    }
}