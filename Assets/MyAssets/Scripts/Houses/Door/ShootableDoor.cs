using Mirror;
using DG.Tweening;
using UnityEngine;
using Unity.VisualScripting;

public class ShootableDoor : Shootable
{

    [SerializeField] private Door door;
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
        else if (!house.isMarked && door.isOutsideDoor)
        {
            if (MafiaHouseTable.instance.selectedHouseMini != null)
            {
                // Mafia currently plan to attack a different house
                PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "I should follow the plan of attack we have at the mafia house...", 1.5f);
            }
            else
            {
                // House is not marked, but the door is protected
                PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "We should make a plan of attack at the mafia house first...", 1.5f);
            }
        }
        else
        {
            // Door is not protected
            {
                KnockDoorDown();
            }
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
