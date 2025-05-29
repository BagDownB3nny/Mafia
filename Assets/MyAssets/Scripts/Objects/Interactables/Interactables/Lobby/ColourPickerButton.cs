using Mirror;
using UnityEngine;

public class ColourPickerButton : Interactable
{
    [SerializeField] public ColourPickerUI colourPickerUI;


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
        colourPickerUI.EnterWindow();
    }
}
