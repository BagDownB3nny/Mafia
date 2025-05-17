using UnityEngine;
using Mirror;

public class InteractableLadder : Interactable
{

    public bool isLocalPlayerOnLadder = false;
    public float timePlayerMountedLadder;
    [SerializeField] Transform ladderTeleportPosition;

    [SyncVar]
    public bool isEnabled = true;

    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
    }

    [Client]
    public override void Interact()
    {
        if (!isEnabled) return;
        if (!isLocalPlayerOnLadder)
        {
            PlayerMountLadder();
        }
        else
        {
            PlayerUnmountLadder();
        }
    }

    public override string GetInteractableText()
    {
        if (!isEnabled) return null;
        if (isLocalPlayerOnLadder)
        {
            return "[E] Jump";
        }
        else
        {
            return "[E] Climb";
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