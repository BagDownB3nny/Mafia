using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class House : NetworkBehaviour
{
    [SerializeField] private List<GameObject> doors;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private Transform doorPositionsHolder;


    [Server]
    public void SpawnDoors(Transform doorsParent)
    {
        foreach (Transform doorPosition in doorPositionsHolder)
        {
            GameObject door = Instantiate(doorPrefab, doorPosition.position, doorPosition.rotation);
            door.transform.localScale = doorPosition.localScale;
            door.transform.SetParent(doorsParent);
            NetworkServer.Spawn(door);
            doors.Add(door);
            RpcSetDoorParent(door, doorsParent);
        }
    }

    [ClientRpc]
    public void RpcSetDoorParent(GameObject door, Transform doorsParent)
    {
        door.transform.SetParent(doorsParent);
        Debug.Log("Setting parent for door");
    }


    [Server]
    public void SetDoorsActive()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
            {
                door.SetActive(true);
                RpcSetDoorActive(door);
            }
        }
    }

    [ClientRpc]
    public void RpcSetDoorActive(GameObject door)
    {
        door.SetActive(true);
    }

    [Server]
    public void SetDoorsInactive()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
            {
                door.SetActive(true);
                RpcSetDoorInactive(door);
            }
        }
    }

    [ClientRpc]
    public void RpcSetDoorInactive(GameObject door)
    {
        door.SetActive(false);
    }
}
