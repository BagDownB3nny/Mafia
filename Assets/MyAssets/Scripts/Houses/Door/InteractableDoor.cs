using Mirror;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class InteractableDoor : Interactable
{

    public bool isOpen = false;
    public bool isBroken = false;
    [SerializeField] private Transform doorOpenPosition;
    [SerializeField] private Transform doorClosedPosition;

    public override void OnHover()
    {
        Highlight();
        string interactableText = isOpen ? "Close the door" : "Open the door";
        PlayerUIManager.instance.SetInteractableText(interactableText);
    }

    public override void OnUnhover()
    {
        Unhighlight();
        PlayerUIManager.instance.ClearInteractableText();
    }

    [Client]
    public override void Interact()
    {
        CmdInteract();
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
