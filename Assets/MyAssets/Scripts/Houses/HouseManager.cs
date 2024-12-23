using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class HouseManager : NetworkBehaviour
{

    [SerializeField] private GameObject housePrefab;
    public static HouseManager instance;

    private List<House> houses = new List<House>();
    [SerializeField] private Transform houseParent;
    [SerializeField] private Transform doorsParent;

    public void Awake()
    {
        if (instance == null)
        {
            Debug.Log("HouseSpawner instance set");
            instance = this;
        }
        else
        {
            Debug.LogWarning("More than one instance of HouseSpawner found!");
            return;
        }
    }

    [Server]
    public void InstantiateHouses()
    {
        int numberOfPlayers = NetworkServer.connections.Count;
        float houseWidth = 12.5f;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector3 housePosition = new Vector3(i * houseWidth, 0, 0);
            GameObject house = Instantiate(housePrefab, housePosition, Quaternion.identity);
            house.transform.SetParent(houseParent);
            NetworkServer.Spawn(house);
            houses.Add(house.GetComponent<House>());
            RpcSetHouseParent(house);

            // Setup doors
            house.GetComponent<House>().SpawnDoors(doorsParent);
        }
    }

    [ClientRpc]
    public void RpcSetHouseParent(GameObject house)
    {
        Debug.Log("Setting parent for house");
        house.transform.SetParent(houseParent);
    }

    [Server]
    public void SetActiveAllDoors()
    {
        foreach (House house in houses)
        {
            house.SetDoorsActive();
        }
    }

    [Server]
    public void SetInactiveAllDoors()
    {
        foreach (House house in houses)
        {
            house.SetDoorsInactive();
        }
    }

    [Server]
    public void ProtectAllHouses()
    {
        foreach (House house in houses)
        {
            house.ActivateProtection();
        }
    }
}
