using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class House : NetworkBehaviour
{
    [SerializeField] private List<Door> doors;
    [SerializeField] private GameObject SeerRoom;

    [SerializeField] public Transform spawnPoint;

    public Vector3 positionRelativeToVillageCenter;

    [SyncVar(hook = nameof(OnPlayerChanged))]

    public Player player;

    [SyncVar]
    public bool isProtected;

    [Server]
    public void AssignPlayer(Player player)
    {
        this.player = player;
        foreach (Door door in doors)
        {
            InteractableDoor interactableDoor = door.GetComponent<InteractableDoor>();
            interactableDoor.AssignAuthority(player);
        }
    }

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
    public void SpawnRoom(RoleName role)
    {
        if (role == RoleName.Seer)
        {
            SeerRoom.SetActive(true);
        }
        RpcSpawnRoom(role);
    }

    [ClientRpc]
    public void RpcSpawnRoom(RoleName role)
    {
        if (role == RoleName.Seer)
        {
            SeerRoom.SetActive(true);
        }
    }

    // Re-assign house authority to the new player
    [Server]
    public void OnPlayerChanged(Player oldPlayer, Player newPlayer)
    {
        if (newPlayer && newPlayer.netIdentity)
        {
            Authority.AssignAuthority(gameObject.GetComponent<NetworkIdentity>(), newPlayer.netIdentity.connectionToClient);
        }
    }
}
