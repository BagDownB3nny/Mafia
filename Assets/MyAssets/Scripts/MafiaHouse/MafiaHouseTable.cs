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
        }
    }

    [Server]
    private void OnPlayerDeath(Player player)
    {
        Debug.Log("PUB SUB SENT DEATH EVENT");
        InteractableVillageHouseMini houseMini = GetHouseMiniFromPlayer(player);
        houseMini.Remove();
    }

    [Server]
    public void SetSelectedHouseMini(InteractableVillageHouseMini houseMini)
    {
        if (selectedHouseMini)
        {
            selectedHouseMini.CmdUnmarkHouse();
        }

        selectedHouseMini = houseMini;
        // Set whiteboard to show selected house
    }


    [Server]
    public void InstantiateHouseMinis()
    {
        List<House> houses = HouseManager.instance.houses;

        foreach (House house in houses)
        {
            Vector3 housePosition = house.positionRelativeToVillageCenter;


            Vector3 houseMiniPositionRelativeToTableCenter = housePosition * 0.02f;

            Vector3 lookAtDirection = (Vector3.zero - housePosition);
            Quaternion houseMiniRotation = Quaternion.LookRotation(lookAtDirection);


            GameObject houseMini = Instantiate(
                houseMiniPrefab,
                transform.position + houseMiniPositionRelativeToTableCenter,
                houseMiniRotation
            );

            houseMini.transform.SetParent(houseMiniParent);

            houseMini.GetComponent<InteractableVillageHouseMini>().linkHouse(house);
            NetworkServer.Spawn(houseMini);
        }
    }

    private void OnSelectedHouseMiniChanged(InteractableVillageHouseMini oldHouseMini, InteractableVillageHouseMini newHouseMini)
    {
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
            if (houseMini.house.player == player)
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
            if (houseMini.house == house)
            {
                return houseMini;
            }
        }
        return null;
    }
}