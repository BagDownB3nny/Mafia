using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MafiaHouseTable : MonoBehaviour
{
    [SerializeField] private GameObject houseMiniPrefab;

    public static MafiaHouseTable instance;
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

            houseMini.transform.SetParent(transform);

            houseMini.GetComponent<InteractableVillageHouseMini>().linkHouse(house);
            NetworkServer.Spawn(houseMini);
        }
    }
}
