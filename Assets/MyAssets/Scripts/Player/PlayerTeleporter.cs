using Mirror;
using UnityEngine;

public class PlayerTeleporter : NetworkBehaviour
{
    public Transform mafiaNightSpawnpoint;
    public Transform executionSpot;
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        if (ExecutionSpot.instance != null)
        {
            executionSpot = ExecutionSpot.instance.transform;
        }
    }



    [Server]
    public void TeleportToSpawn()
    {
        House house = player.house;
        TeleportPlayer(house.spawnPoint.position);
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
}
