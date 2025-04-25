using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class House : NetworkBehaviour
{

    [Header("House object references")]
    [SerializeField] private List<Door> doors;
    [SerializeField] private InteractableDoor trapDoor;
    [SerializeField] private GameObject SeerRoom;
    [SerializeField] public TMPro.TextMeshProUGUI namePlateText;

    public Transform spawnPoint;
    public Transform tunnelTeleporterPosition;


    [Header("Highlighting")]
    [SerializeField] private GameObject highlightableHouse;

    public Vector3 positionRelativeToVillageCenter;


    [Header("Player")]

    [SyncVar(hook = nameof(OnPlayerChanged))]
    public Player player;

    [Header("Internal params")]

    [SyncVar]
    public bool isMarked;

    public void OnDestroy()
    {
        if (HouseManager.instance != null)
        {
            HouseManager.instance.houses.Remove(this);
        }
    }

    [Server]
    public void AssignPlayer(Player player)
    {
        if (player.isLocalPlayer)
        {
            MafiaHouseTeleporter.instance.SetLocalPlayerDefaultTeleportPoint(tunnelTeleporterPosition);
        }
        this.player = player;
        Debug.Log($"Assigned player {player.steamUsername} to house {netId}");
        foreach (Door door in doors)
        {
            InteractableDoor interactableDoor = door.GetComponent<InteractableDoor>();
            interactableDoor.AssignAuthority(player);
        }
    }

    [Client]
    public void OnPlayerChanged(Player oldValue, Player newValue)
    {
        namePlateText.text = newValue.steamUsername;
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
    public void Mark()
    {
        isMarked = true;
        HighlightForMafia();
    }

    [Server]
    public void Unmark()
    {
        isMarked = false;
        UnhighlightForMafia();
    }

    [Server]
    public void HighlightForMafia()
    {
        RpcHighlightForMafia();
    }

    [ClientRpc]
    public void RpcHighlightForMafia()
    {
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (localPlayer.role == RoleName.Mafia)
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
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (localPlayer.role == RoleName.Mafia && !localPlayer.isDead)
        {
            SetHighlight(false);
        }
    }

    [Server]
    public void HighlightForGhosts()
    {
        RpcHighlightForGhosts();
    }

    [ClientRpc]
    public void RpcHighlightForGhosts()
    {
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (localPlayer.isDead)
        {
            SetHighlight(true);
        }
    }

    [Server]
    public void UnhighlightForGhosts()
    {
        RpcUnhighlightForGhosts();
    }

    [ClientRpc]
    public void RpcUnhighlightForGhosts()
    {
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (localPlayer.isDead)
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
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (localPlayer == this.player)
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
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (localPlayer == this.player)
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

    [Server]
    public void DoorDestroyed(Door door)
    {
        doors.Remove(door);

        // Because the basement door doesnt count, we need the lvl 1
        // doors to be destroyed for the house to be considered destroyed
        // since the basement door cannot be accessed by non-mafia
        if (doors.Count == 2)
        {
            Debug.Log("House destroyed");
            // Publish event to notify that the house is destroyed
            PubSub.Publish(PubSubEvent.HouseDestroyed, this);
        }
    }
}
