using Mirror;
using UnityEngine;

public class ColourPickerButton : Interactable
{

    private bool isColourButtonWindowActive = false;
    [SerializeField] public GameObject colourButtonWindow;


    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
        // test
    }

    public override string GetInteractableText()
    {
        return "Change colour";
    }

    public override void Interact()
    {
        colourButtonWindow.SetActive(true);
        isColourButtonWindowActive = true;
        PlayerCamera.instance.EnterCursorMode();
    }
}
