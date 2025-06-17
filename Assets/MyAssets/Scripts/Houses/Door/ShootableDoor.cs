using Mirror;
using DG.Tweening;
using UnityEngine;
using Unity.VisualScripting;

public class ShootableDoor : Shootable
{

    [SerializeField] private Door door;
    [SerializeField] private Transform doorKnockedPosition;


    // The return value indicates if a shot was successfully fired
    [Server]
    public override bool OnShot(NetworkConnectionToClient shooter)
    {
        if (door.isKnockedDown) return true;
        House house = GetComponent<Door>().house;
        HouseProtectionSigil houseProtectionSigil = house.GetComponentInChildren<HouseProtectionSigil>(includeInactive: true);
        if (!house.isMarked && door.isOutsideDoor)
        {
            if (TargetDummyManager.instance.cursedTargetDummy != null)
            {
                // Mafia currently plan to attack a different house
                PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "I should follow the plan of attack we have at the mafia house...", 1.5f);
                return false;
            }
            else
            {
                // House is not marked
                PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "We should make a plan of attack at the mafia house first...", 1.5f);
                return false;
            }
        }
        else if (houseProtectionSigil.isMarked && door.isOutsideDoor)
        {
            // Door is protected, not knocked down
            PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "This house is protected by the guardian!", 1.5f);
            return true;
        }
        else
        {
            // Door is not protected
            {
                KnockDoorDown();
                return true;
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

        door.isKnockedDown = true;
    }

    [ClientRpc]
    private void RpcKnockDoorDown()
    {
        float animationDuration = 0.7f;
        transform.DOMove(doorKnockedPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorKnockedPosition.rotation, animationDuration).SetEase(Ease.InQuad);
    }
}
