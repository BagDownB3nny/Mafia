using UnityEngine;
public class InteractablePlayer : Interactable
{
    public override void Interact()
    {
        // Shouldnt do anything, since the method to invoke is in the role scripts
    }

    public override string GetInteractableText()
    {
        return null;
    }

    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Guardian, RoleName.Seer };
    }
}