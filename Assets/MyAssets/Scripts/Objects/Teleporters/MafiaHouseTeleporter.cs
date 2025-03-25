using UnityEngine;

public class MafiaHouseTeleporter : MonoBehaviour
{
    public static MafiaHouseTeleporter instance;
    public Transform destination;

    private Transform defaultLocalPlayerTeleportPoint;
    private Transform localPlayerTeleportPoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetLocalPlayerDefaultTeleportPoint(Transform point)
    {
        defaultLocalPlayerTeleportPoint = point;
    }

    public void SetLocalPlayerTeleportPoint(Transform point)
    {
        localPlayerTeleportPoint = point;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponentInParent<Player>();
            if (player.isLocalPlayer)
            {
                PlayerTeleporter playerTeleporter = player.GetComponent<PlayerTeleporter>();
                playerTeleporter.ClientTeleportPlayer(localPlayerTeleportPoint.position, localPlayerTeleportPoint.transform.rotation);
                localPlayerTeleportPoint = defaultLocalPlayerTeleportPoint;
            }
        }
    }
}
