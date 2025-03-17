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

    public void RemoveHouseMini()
    {
        
    }
}