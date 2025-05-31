using UnityEngine;

public class LobbySettingsButton : Interactable
{
    [SerializeField] private Menu lobbySettingsMenu;


    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
    }

    public override string GetInteractableText()
    {
        return "Change game settings";
    }

    public override void Interact()
    {
        lobbySettingsMenu.Open();
        PlayerCamera.instance.EnterCursorMode();
    }
}
