using Mirror;
using UnityEngine;
using DG.Tweening;

public class InteractableDoor : Interactable
{

    [SerializeField] private Door door;

    public bool isOpen = false;
    public bool isBroken = false;

    public readonly SyncList<uint> authorisedPlayers = new SyncList<uint>();
    [SerializeField] private Transform doorOpenPosition;
    [SerializeField] private Transform doorClosedPosition;

    public void Start()
    {
        Unhighlight();
    }

    [Server]
    public void AssignAuthority(Player player)
    {
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        authorisedPlayers.Add(player.netId);
    }

    [Server]
    public void RemoveAuthority(Player player)
    {
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        authorisedPlayers.Remove(player.netId);
    }

    [Client]
    public override void OnHover()
    {
        Highlight();
        uint playerNetId = PlayerManager.instance.localPlayer.netId;
        Role playerRoleScript = PlayerManager.instance.localPlayer.GetRoleScript();
        if (authorisedPlayers.Contains(playerNetId))
        {
            string interactableText = isOpen ? "Close the door" : "Open the door";
            PlayerUIManager.instance.SetInteractableText(interactableText);
        }
        else if (playerRoleScript.IsAbleToInteractWithDoors && door.isOutsideDoor)
        {
            string interactableText = playerRoleScript.InteractWithDoorText;
            PlayerUIManager.instance.SetInteractableText(interactableText);
        }
        else
        {
            PlayerUIManager.instance.SetInteractableText("Door is locked");
        }
    }

    [Client]
    public override void OnUnhover()
    {
        Unhighlight();
        PlayerUIManager.instance.ClearInteractableText();
    }

    [Client]
    public override void Interact()
    {
        uint playerNetId = PlayerManager.instance.localPlayer.netId;
        if (authorisedPlayers.Contains(playerNetId))
        {
            CmdInteract();
        }
        else
        {
            PlayerUIManager.instance.SetInteractableText("Door is locked");
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdInteract()
    {
        if (isOpen)
        {
            RpcCloseDoor();
        }
        else
        {
            RpcOpenDoor();
        }
    }

    [Server]
    public void OpenDoor()
    {
        isOpen = true;
        float animationDuration = 0.7f;
        transform.DOMove(doorOpenPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorOpenPosition.rotation, animationDuration).SetEase(Ease.InQuad);
        RpcOpenDoor();
    }

    [ClientRpc]
    public void RpcOpenDoor()
    {
        isOpen = true;
        float animationDuration = 0.7f;

        if (doorOpenPosition == null)
        {
            Debug.LogError("Door open position is null");
            return;
        }
        transform.DOMove(doorOpenPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorOpenPosition.rotation, animationDuration).SetEase(Ease.InQuad);
    }

    [Server]
    public void CloseDoor()
    {
        isOpen = false;
        float animationDuration = 0.7f;
        transform.DOMove(doorClosedPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorClosedPosition.rotation, animationDuration).SetEase(Ease.InQuad);
        RpcCloseDoor();
    }

    [ClientRpc]
    public void RpcCloseDoor()
    {
        isOpen = false;
        float animationDuration = 0.7f;

        if (doorClosedPosition == null)
        {
            Debug.LogError("Door closed position is null");
            return;
        }
        transform.DOMove(doorClosedPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorClosedPosition.rotation, animationDuration).SetEase(Ease.InQuad);
    }
}
