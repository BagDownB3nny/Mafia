using Mirror;
using DG.Tweening;
using UnityEngine;

public class ShootableDoor : Shootable
{
    [SerializeField] private Transform doorKnockedPosition;

    [Server]
    public override void OnShot(NetworkConnectionToClient shooter)
    {
        House house = GetComponent<Door>().house;
        HouseProtectionSigil houseProtectionSigil = house.GetComponentInChildren<HouseProtectionSigil>(includeInactive: true);
        if (houseProtectionSigil.isMarked)
        {
            // Door is protected, not knocked down
            PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "This door is protected!", 1.5f);
        }
        else
        {
            KnockDoorDown();
        }
    }

    [Server]
    private void KnockDoorDown()
    {
        RpcKnockDoorDown();

        // Tell house that a door has been knocked down
        Door door = GetComponent<Door>();
        House house = door.house;
        house.DoorDestroyed(door);
    }

    [ClientRpc]
    private void RpcKnockDoorDown()
    {
        float animationDuration = 0.7f;
        transform.DOMove(doorKnockedPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorKnockedPosition.rotation, animationDuration).SetEase(Ease.InQuad);
    }
}
