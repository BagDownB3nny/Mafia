using UnityEngine;
using Mirror;

public class LadderColliderTeleporter : NetworkBehaviour
{
    [SerializeField] Transform playerPosition;
    [SerializeField] InteractableLadder ladder;

    public void OnTriggerEnter(Collider other)
    {
        if (Time.time - ladder.timePlayerMountedLadder < 0.05f) return;
        if (other.CompareTag("Player") && ladder.isLocalPlayerOnLadder)
        {
            Player player = NetworkClient.localPlayer.GetComponent<Player>();
            player.GetComponent<PlayerTeleporter>().ClientTeleportPlayer(playerPosition.position, playerPosition.rotation);
            player.GetComponent<PlayerMovement>().ChangeToNormalMovement();
            ladder.isLocalPlayerOnLadder = false;
        }
    }
}
