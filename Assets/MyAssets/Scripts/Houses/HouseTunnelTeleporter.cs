using UnityEngine;
using Mirror;

public class HouseTunnelTeleporter : NetworkBehaviour
{
    public Transform exitPoint;
    public HouseTunnelTeleporter target;

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
            }
        }
    }

}
