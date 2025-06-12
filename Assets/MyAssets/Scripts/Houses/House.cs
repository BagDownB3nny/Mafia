using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class House : NetworkBehaviour
{


    [Header("House object references")]
    [SerializeField] private List<Door> doors;
    [SerializeField] private Door trapDoor;
    [SerializeField] private InteractableLadder ladder;
    [SerializeField] TMPro.TextMeshProUGUI namePlateText;
    [Header("Role rooms")]
    [SerializeField] private GameObject SeerRoom;
    [SerializeField] private GameObject MediumRoom;
    [SerializeField] private GameObject MafiaRoom;

    public Transform spawnPoint;
    public Transform tunnelTeleporterPosition;

    [Header("Target dummy")]
    [SerializeField] public TargetDummy targetDummy;


    [Header("Highlighting")]
    [SerializeField] private GameObject highlightableHouse;

    public Vector3 positionRelativeToVillageCenter;


    [Header("Player")]

    [SyncVar(hook = nameof(OnPlayerChanged))]
    public Player player;

    [Header("Internal params")]

    [SyncVar]
    public bool isMarked;

    public void EnableTrapdoor()
    {
        // trapDoor.isEnabled = true;
        // ladder.isEnabled = true;
    }

    [Server]
    public void AssignPlayer(Player player)
    {
        if (player.isLocalPlayer)
        {
            MafiaHouseTeleporter.instance.SetLocalPlayerDefaultTeleportPoint(tunnelTeleporterPosition);
        }
        this.player = player;
        targetDummy.SetActive(true);
        targetDummy.SetLinkedPlayer(player);
        foreach (Door door in doors)
        {
            InteractableDoor interactableDoor = door.GetComponent<InteractableDoor>();
            interactableDoor.AssignAuthority(player);
        }
    }

    [Client]
    public void OnPlayerChanged(Player oldValue, Player newValue)
    {
        if (newValue == null)
        {
            Debug.LogError($"Server SyncVar happens before client Var is created. " +
                $"Player is null for house {netId}");
            return;
        }
        SetNameplateText(newValue.steamUsername);
    }

    [Client]
    public void SetNameplateText(string text)
    {
        namePlateText.text = text;
    }

    [Client]
    public void SetDoorToGhostLayer()
    {
        foreach (Door door in doors)
        {
            door.gameObject.layer = LayerName.GhostPassable.Index();
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

    [TargetRpc]
    public void RpcSetDoorActive(NetworkConnectionToClient target, GameObject door)
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

    [TargetRpc]
    public void RpcSetDoorInactive(NetworkConnectionToClient target, GameObject door)
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
        Player localPlayer = NetworkClient.localPlayer.GetComponent<Player>();
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
        Player localPlayer = NetworkClient.localPlayer.GetComponent<Player>();
        bool isDead = localPlayer.GetComponent<PlayerDeath>().isDead;
        if (localPlayer.role == RoleName.Mafia && !isDead)
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
        Player localPlayer = NetworkClient.localPlayer.GetComponent<Player>();
        bool isDead = localPlayer.GetComponent<PlayerDeath>().isDead;
        if (isDead)
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
        Player localPlayer = NetworkClient.localPlayer.GetComponent<Player>();
        bool isDead = localPlayer.GetComponent<PlayerDeath>().isDead;
        if (isDead)
        {
            SetHighlight(false);
        }
    }

    [Server]
    public void HighlightForOwner()
    {
        if (player != null && player.connectionToClient != null)
        {
            RpcHighlightForOwner(player.connectionToClient);
        }
    }

    [TargetRpc]
    public void RpcHighlightForOwner(NetworkConnectionToClient target)
    {
        SetHighlight(true);
    }

    [Server]
    public void UnhighlightForOwner()
    {
        if (player != null && player.connectionToClient != null)
        {
            RpcUnhighlightForOwner(player.connectionToClient);
        }
    }

    [TargetRpc]
    public void RpcUnhighlightForOwner(NetworkConnectionToClient target)
    {
        SetHighlight(false);
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
        // trapDoor.GetComponent<InteractableDoor>().RemoveAuthority(player);
        // ladder.isEnabled = false;
    }

    [Server]
    public void UnlockTrapDoor()
    {
        // trapDoor.GetComponent<InteractableDoor>().AssignAuthority(player);
        // ladder.isEnabled = true;
    }

    [Server]
    public void SpawnRoom(RoleName role)
    {
        if (role == RoleName.Seer)
        {
            SeerRoom.SetActive(true);
        }
        else if (role == RoleName.Medium)
        {
            MediumRoom.SetActive(true);
        }
        else if (role == RoleName.Mafia)
        {
            MafiaRoom.SetActive(true);
        }
        RpcSpawnRoleRoom(role);
    }

    [ClientRpc]
    public void RpcSpawnRoleRoom(RoleName role)
    {
        if (role == RoleName.Seer)
        {
            SeerRoom.SetActive(true);
        }
        else if (role == RoleName.Medium)
        {
            MediumRoom.SetActive(true);
        }
        else if (role == RoleName.Mafia)
        {
            MafiaRoom.SetActive(true);
        }
    }

    [Server]
    public void DoorDestroyed(Door door)
    {
        // If outside door is destroyed, the rest of the house is accessible with a gun
        if (door.isOutsideDoor)
        {
            Debug.Log("House destroyed");
            // Publish event to notify that the house is destroyed
            PubSub.Publish(PubSubEvent.HouseDestroyed, this);
        }
    }
}
