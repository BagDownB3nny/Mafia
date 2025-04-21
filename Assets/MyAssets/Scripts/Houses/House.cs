using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class House : NetworkBehaviour
{

    [SerializeField] private List<Door> doors;
    [SerializeField] private InteractableDoor trapDoor;
    [SerializeField] private GameObject SeerRoom;

    [SerializeField] public Transform spawnPoint;
    [SerializeField] public Transform tunnelTeleporterPosition;


    [Header("Highlighting")]
    [SerializeField] private GameObject highlightableHouse;

    public Vector3 positionRelativeToVillageCenter;

    [SyncVar(hook = nameof(OnPlayerChanged))]

    public Player player;

    [SyncVar]
    public bool isProtected;

    [Server]
    public void AssignPlayer(Player player)
    {
        if (player.isLocalPlayer)
        {
            MafiaHouseTeleporter.instance.SetLocalPlayerDefaultTeleportPoint(tunnelTeleporterPosition);
        }
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
        UnhighlightForMafia();
    }

    [Server]
    public void DeactivateProtection()
    {
        isProtected = false;
        HighlightForMafia();
    }

    [Server]
    public void HighlightForMafia()
    {
        RpcHighlightForMafia();
    }

    [ClientRpc]
    public void RpcHighlightForMafia()
    {
        Player player = PlayerManager.instance.localPlayer;
        if (player.role == RoleName.Mafia)
        {
            SetHighlight(true);
        }
    }

    [Server]
    public void UnhighlightForMafia()
    {
        RpcUnhighlightForMafia();
    }

    [ClientRpc]
    public void RpcUnhighlightForMafia()
    {
        Player player = PlayerManager.instance.localPlayer;
        if (player.role == RoleName.Mafia)
        {
            SetHighlight(false);
        }
    }

    [Server]
    public void HighlightForOwner()
    {
        RpcHighlightForOwner();
    }

    [ClientRpc]
    public void RpcHighlightForOwner()
    {
        Player player = PlayerManager.instance.localPlayer;
        if (player == this.player)
        {
            SetHighlight(true);
        }
    }

    [Server]
    public void UnhighlightForOwner()
    {
        RpcUnhighlightForOwner();
    }

    [ClientRpc]
    public void RpcUnhighlightForOwner()
    {
        Player player = PlayerManager.instance.localPlayer;
        if (player == this.player)
        {
            SetHighlight(false);
        }
    }

    [Client]
    public void SetHighlight(bool highlight)
    {
        if (highlight)
        {
            highlightableHouse.GetComponent<Outline>().enabled = true;
        }
        else
        {
            highlightableHouse.GetComponent<Outline>().enabled = false;
        }
    }

    [Server]
    public void LockTrapDoor()
    {
        trapDoor.RemoveAuthority(player);
    }

    [Server]
    public void UnlockTrapDoor()
    {
        trapDoor.AssignAuthority(player);
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
