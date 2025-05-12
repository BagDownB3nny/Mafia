using Mirror;
using UnityEngine;

public class PlayerTeleporter : NetworkBehaviour
{

    [Header("Teleport locations")]
    public Transform mafiaNightSpawnpoint;
    private Transform mafiaHouseTunnel;
    public Transform executionSpot;

    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        if (ExecutionSpot.instance != null)
        {
            executionSpot = ExecutionSpot.instance.transform;
        }
        mafiaHouseTunnel = MafiaHouseTeleporter.instance.destination;
    }



    [Server]
    public void TeleportToSpawn()
    {
        House house = player.house;
        TeleportPlayer(house.spawnPoint.position);
    }

    [Server]
    public void TeleportToSpectatorSpawn()
    {
        if (SpectatorSpawn.spectatorSpawnpoint != null)
        {
            TeleportPlayer(SpectatorSpawn.spectatorSpawnpoint.position);
        }
        else
        {
            Debug.LogError("Spectator spawnpoint not set!");
        }
    }

    [Server]
    public void TeleportToNightSpawn()
    {
        if (player.role == RoleName.Mafia)
        {
            TeleportPlayer(mafiaNightSpawnpoint.position);
        }
        else
        {
            TeleportToSpawn();
        }
    }

    [Server]
    public void TeleportToExecutionSpot()
    {
        TeleportPlayer(executionSpot.position);
    }


    [Server]
    public void TeleportPlayer(Vector3 position)
    {
        RpcTeleportPlayer(position);
    }

    [ClientRpc]
    private void RpcTeleportPlayer(Vector3 position)
    {
        NetworkTransformBase networkTransform = GetComponent<NetworkTransformBase>();
        networkTransform.OnTeleport(position);
        Physics.SyncTransforms();
    }

    [Client]
    public void ClientTeleportPlayer(Vector3 position, Quaternion rotation)
    {
        NetworkTransformBase networkTransform = GetComponent<NetworkTransformBase>();
        networkTransform.OnTeleport(position);
        Physics.SyncTransforms();
    }

    [Client]
    public void TeleportToMafiaHouseTunnel()
    {
        ClientTeleportPlayer(mafiaHouseTunnel.position, mafiaHouseTunnel.transform.rotation);
    }
}
