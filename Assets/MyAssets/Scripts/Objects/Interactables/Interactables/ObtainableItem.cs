using UnityEngine;
using Mirror;

public class ObtainableItem : Interactable
{

    public bool isInWorld = true;

    [SerializeField] public GameObject itemVisuals;
    [SerializeField] public Items item;
    [SerializeField] public RoleName[] rolesThatCanPickUp;
    
    [Command (requiresAuthority = false)]
    public void CmdRemoveFromWorld()
    {
        itemVisuals.SetActive(false);
        isInWorld = false;
    }

    public override RoleName[] GetRolesThatCanInteract()
    {
        return rolesThatCanPickUp;
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
