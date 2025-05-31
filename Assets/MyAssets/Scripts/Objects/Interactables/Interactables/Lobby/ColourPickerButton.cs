using Mirror;
using UnityEngine;

public class ColourPickerButton : Interactable
{
    public ColourPickerMenu colourPickerMenu;


    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
    }

    public override string GetInteractableText()
    {
        return "Change colour";
    }

    public override void Interact()
    {
        colourPickerMenu.Open();
    }
}
