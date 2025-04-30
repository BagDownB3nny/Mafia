using Mirror;
using UnityEngine;

// NOT CURRENTLY IN USE, SCRIPT HERE IN CASE WE DECIDE TO USE AN INVENTORY SYSTEM

public abstract class Item : NetworkBehaviour
{
    public abstract bool isAbleToInteractWithDoors { get; }
    public abstract bool isAbleToInteractWithPlayers { get; }

    public bool IsAbleToInteractWithInteractable(Interactable interactable)
    {
        if (interactable is InteractableDoor)
        {
            return isAbleToInteractWithDoors;
        }
        else if (interactable is InteractablePlayer)
        {
            return isAbleToInteractWithPlayers;
        }
        return false;
    }

    public virtual void HandleDoorInteraction(InteractableDoor door)
    {
        throw new System.NotImplementedException("HandleDoorInteraction not implemented");
    }

    public virtual void HandlePlayerInteraction(InteractablePlayer player)
    {
        throw new System.NotImplementedException("HandlePlayerInteraction not implemented");
    }
}
