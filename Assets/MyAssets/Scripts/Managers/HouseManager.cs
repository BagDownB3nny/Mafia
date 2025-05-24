using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HouseManager : NetworkBehaviour
{

    [SerializeField] private GameObject housePrefab;
    public static HouseManager instance;

    public List<House> houses = new();
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

    public House GetHouseByNetId(uint netId)
    {
        if (NetworkServer.spawned.ContainsKey(netId))
        {
            return NetworkServer.spawned[netId].GetComponent<House>();
        }
        else if (NetworkClient.spawned.ContainsKey(netId))
        {
            return NetworkClient.spawned[netId].GetComponent<House>();
        }
        else
        {
            return null;
        }
    }

    [Server]
    public void InstantiateHouses()
    {
        // int numberOfPlayers = 16;
        int numberOfPlayers = NetworkServer.connections.Count;
        float angleStep = 360f / numberOfPlayers;
        // Different radiuses for different number of players
        float radius;
        if (numberOfPlayers <= 12)
        {
            radius = 40;
        }
        else
        {
            radius = 40;
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
        }
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
            house.Mark();
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
            house.UnhighlightForOwner();
        }
    }

    [Server]
    public void HighlightMediumHouseForGhosts()
    {
        foreach (House house in houses)
        {
            Player owner = house.player;
            if (owner != null && owner.GetRole() == RoleName.Medium)
            {
                house.HighlightForGhosts();
            }
        }
    }

    [Server]
    public void UnhighlightMediumHouseForGhosts()
    {
        foreach (House house in houses)
        {
            Player owner = house.player;
            if (owner != null && owner.GetRole() == RoleName.Medium)
            {
                house.UnhighlightForGhosts();
            }
        }
    }
}
