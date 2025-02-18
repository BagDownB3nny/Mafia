using Mirror;
using DG.Tweening;
using UnityEngine;

public class ShootableDoor : Shootable
{
    [SerializeField] private Transform doorKnockedPosition;

    [Server]
    public override void OnShot(NetworkConnectionToClient shooter)
    {
        InteractableDoor interactableDoor = GetComponent<InteractableDoor>();
        House house = GetComponent<Door>().house;
        if (house.isProtected)
        {
            PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "This house is protected!", 1.5f);
            return;
        }
        if (!interactableDoor.isOpen)
        {
            KnockDoorDown();
        }
    }

    [Server]
    private void KnockDoorDown()
    {


        // Swing door down to the ground, with the door base as the pivot point
        float animationDuration = 0.7f;
        transform.DOMove(doorKnockedPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorKnockedPosition.rotation, animationDuration).SetEase(Ease.InQuad);
        RpcKnockDoorDown();
    }

    [ClientRpc]
    private void RpcKnockDoorDown()
    {
        float animationDuration = 0.7f;
        transform.DOMove(doorKnockedPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorKnockedPosition.rotation, animationDuration).SetEase(Ease.InQuad);
    }
}
