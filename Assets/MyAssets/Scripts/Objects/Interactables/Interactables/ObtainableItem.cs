using UnityEngine;
using Mirror;

public abstract class ObtainableItem : Interactable
{

    public bool isInWorld = true;

    [SerializeField] public GameObject itemVisuals;
    [SerializeField] public Items item;
    
    [Command (requiresAuthority = false)]
    public void CmdRemoveFromWorld()
    {
        itemVisuals.SetActive(false);
        isInWorld = false;
    }

    [Command (requiresAuthority = false)]
    public void CmdAddToWorld()
    {
        itemVisuals.SetActive(true);
        isInWorld = true;
    }

    [Client]
    public override void Interact()
    {
        // Logic handled in PlayerItemGrabbingAction
    }
}
