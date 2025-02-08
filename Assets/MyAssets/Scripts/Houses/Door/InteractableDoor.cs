using Mirror;
using UnityEngine;
using DG.Tweening;

public class InteractableDoor : Interactable
{

    public bool isOpen = false;
    public bool isBroken = false;
    [SerializeField] private Transform doorOpenPosition;
    [SerializeField] private Transform doorClosedPosition;

    public void Start()
    {
        Unhighlight();
    }

    [Client]
    public override void OnHover()
    {
        Highlight();
        if (isOwned)
        {
            string interactableText = isOpen ? "Close the door" : "Open the door";
            PlayerUIManager.instance.SetInteractableText(interactableText);
        }
        else
        {
            PlayerUIManager.instance.SetInteractableText("This door is protected");
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
        if (isOwned)
        {
            CmdInteract();
        }
        else
        {
            PlayerUIManager.instance.SetInteractableText("You are unable to get past the protection");
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
