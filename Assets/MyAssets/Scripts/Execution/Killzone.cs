using UnityEngine;
using Mirror;

public class Killzone : MonoBehaviour
{

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInChildren<ShootablePlayer>().SetDeath();
        }
    }
}
