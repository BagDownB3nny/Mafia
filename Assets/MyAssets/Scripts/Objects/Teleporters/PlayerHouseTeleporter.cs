using UnityEngine;

public class PlayerHouseTeleporter : MonoBehaviour
{
    [SerializeField] private Transform destination;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponentInParent<Player>();
            if (player.isLocalPlayer)
            {
                PlayerTeleporter playerTeleporter = player.GetComponent<PlayerTeleporter>();
                playerTeleporter.TeleportToMafiaHouseTunnel();
                MafiaHouseTeleporter.instance.SetLocalPlayerTeleportPoint(destination.transform);
            }
        }
    }
}
