using UnityEngine;

public class LadderCollider : MonoBehaviour
{

    [SerializeField] InteractableLadder ladder;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && ladder.isLocalPlayerOnLadder)
        {
            Player player = PlayerManager.instance.localPlayer;
            player.GetComponent<PlayerMovement>().ChangeToNormalMovement();
            ladder.isLocalPlayerOnLadder = false;
        }
    }
}
