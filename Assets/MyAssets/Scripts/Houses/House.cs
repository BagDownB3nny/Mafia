using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class House : NetworkBehaviour
{
    [SerializeField] private List<Door> doors;
    [SerializeField] private GameObject SeerRoom;

    [SerializeField] public Transform spawnPoint;

    [SyncVar]

    public Player player;

    [SyncVar]
    public bool isProtected;

    [Server]
    public void CloseAllDoors()
    {
        foreach (Door door in doors)
        {
            InteractableDoor interactableDoor = door.GetComponent<InteractableDoor>();
            interactableDoor.CloseDoor();
        }
    }

    [ClientRpc]
    public void RpcSetDoorActive(GameObject door)
    {
        door.SetActive(true);
    }

    [Server]
    public void OpenAllDoors()
    {
        foreach (Door door in doors)
        {
            InteractableDoor interactableDoor = door.GetComponent<InteractableDoor>();
            interactableDoor.OpenDoor();
        }
    }

    [ClientRpc]
    public void RpcSetDoorInactive(GameObject door)
    {
        door.SetActive(false);
    }

    [Server]
    public void ActivateProtection()
    {
        isProtected = true;
    }

    [Server]
    public void DeactivateProtection()
    {
        isProtected = false;
    }

    [Server]
    public void SpawnRoom(Roles role)
    {
        if (role == Roles.Seer)
        {
            SeerRoom.SetActive(true);
        }
        RpcSpawnRoom(role);
    }

    [ClientRpc]
    public void RpcSpawnRoom(Roles role)
    {
        if (role == Roles.Seer)
        {
            SeerRoom.SetActive(true);
        }
    }
}
