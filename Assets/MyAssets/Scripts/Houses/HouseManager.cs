using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HouseSpawner : NetworkBehaviour
{

    [SerializeField] private GameObject housePrefab;
    public static HouseSpawner instance;

    private List<House> houses = new List<House>();

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
            GameObject house = Instantiate(housePrefab, new Vector3(i * houseWidth, 0, 0), Quaternion.identity);
            house.transform.SetParent(transform);

            NetworkServer.Spawn(house);
        }
    }

    public void SetActiveAllDoors()
    {
        foreach (House house in houses)
        {
            house.SetDoorsActive();
        }
    }
}
