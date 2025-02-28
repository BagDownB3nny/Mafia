using UnityEngine;
using Mirror;
using UnityEditor.ShaderGraph.Internal;

public class InteractableLadder : Interactable
{

    public bool isLocalPlayerOnLadder = false;
    public float timePlayerMountedLadder;
    [SerializeField] Transform ladderTeleportPosition;

    [Client]
    public override void OnHover()
    {
        PlayerUIManager.instance.SetInteractableText("Climb the ladder");
    }

    [Client]
    public override void OnUnhover()
    {
        PlayerUIManager.instance.ClearInteractableText();
    }

    [Client]
    public override void Interact()
    {
        if (!isLocalPlayerOnLadder)
        {
            PlayerMountLadder();
        }
        else
        {
            PlayerUnmountLadder();
        }
    }

    [Client]
    private void PlayerMountLadder()
    {
        timePlayerMountedLadder = Time.time;
        isLocalPlayerOnLadder = true;
        Player localPlayer = PlayerManager.instance.localPlayer;

        localPlayer.GetComponent<PlayerMovement>().ChangeToLadderMovement();

        Vector3 teleportPosition = ladderTeleportPosition.position;
        teleportPosition.y = localPlayer.transform.position.y;
        Quaternion teleportRotation = ladderTeleportPosition.rotation;

        localPlayer.GetComponent<PlayerTeleporter>().ClientTeleportPlayer(teleportPosition, teleportRotation);
    }

    [Client]
    private void PlayerUnmountLadder()
    {
        isLocalPlayerOnLadder = false;
        Player localPlayer = PlayerManager.instance.localPlayer;

        localPlayer.GetComponent<PlayerMovement>().ChangeToNormalMovement();
    }
}