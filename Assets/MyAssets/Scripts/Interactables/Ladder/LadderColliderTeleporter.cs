using UnityEngine;

public class LadderColliderTeleporter : MonoBehaviour
{
    [SerializeField] Transform playerPosition;
    [SerializeField] InteractableLadder ladder;

    public void OnTriggerEnter(Collider other)
    {
        if (Time.time - ladder.timePlayerMountedLadder < 0.5f) return;
        if (other.CompareTag("Player") && ladder.isLocalPlayerOnLadder)
        {
            Player player = PlayerManager.instance.localPlayer;
            player.GetComponent<PlayerTeleporter>().ClientTeleportPlayer(playerPosition.position, playerPosition.rotation);
            player.GetComponent<PlayerMovement>().ChangeToNormalMovement();
            ladder.isLocalPlayerOnLadder = false;
        }
    }
}
