using UnityEngine;
using Mirror;

public class LadderCollider : NetworkBehaviour
{

    [SerializeField] InteractableLadder ladder;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && ladder.isLocalPlayerOnLadder)
        {
            Player player = NetworkClient.localPlayer.GetComponent<Player>();
            player.GetComponent<PlayerMovement>().ChangeToNormalMovement();
            ladder.isLocalPlayerOnLadder = false;
        }
    }
}
