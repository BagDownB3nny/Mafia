using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HouseManager : NetworkBehaviour
{

    [SerializeField] private GameObject housePrefab;
    public static HouseManager instance;

    public List<House> houses = new List<House>();
    [SerializeField] private Transform houseParent;
    [SerializeField] private Transform doorsParent;

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
    public void InstantiateHouses()
    {
        // int numberOfPlayers = NetworkServer.connections.Count;
        int numberOfPlayers = 16;
        float angleStep = 360f / numberOfPlayers;
        // Different radiuses for different number of players
        float radius;
        if (numberOfPlayers <= 12)
        {
            radius = 30;
        }
        else
        {
            radius = 35;
        }
        for (int i = 0; i < numberOfPlayers; i++)
        {
            float angle = i * angleStep; // Angle for the current player
            float angleInRadians = angle * Mathf.Deg2Rad;

            // houses should be spawned in a circle around the center of the map
            Vector3 housePosition = new Vector3(
                Mathf.Cos(angleInRadians) * radius,
                0,
                Mathf.Sin(angleInRadians) * radius
            );

            // Calculate the rotation to face the center
            Vector3 directionToCenter = (Vector3.zero - housePosition).normalized;
            Quaternion houseRotation = Quaternion.LookRotation(directionToCenter);

            GameObject house = Instantiate(housePrefab, housePosition, houseRotation);
            house.transform.SetParent(houseParent);
            house.GetComponent<House>().positionRelativeToVillageCenter = transform.InverseTransformPoint(house.transform.position);

            NetworkServer.Spawn(house);
            houses.Add(house.GetComponent<House>());
            RpcSetHouseParent(house);
        }
    }

    [ClientRpc]
    public void RpcSetHouseParent(GameObject house)
    {
        house.transform.SetParent(houseParent);
    }

    [Server]
    public void CloseAllDoors()
    {
        foreach (House house in houses)
        {
            house.CloseAllDoors();
        }
    }

    [Server]
    public void OpenAllDoors()
    {
        foreach (House house in houses)
        {
            house.OpenAllDoors();
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

    [Server]
    public void HighlightHousesForOwners()
    {
        foreach (House house in houses)
        {
            house.HighlightForOwner();
        }
    }

    [Server]
    public void UnhighlightHousesForOwners()
    {
        foreach (House house in houses)
        {
            house.HighlightForOwner();
        }
    }
}
