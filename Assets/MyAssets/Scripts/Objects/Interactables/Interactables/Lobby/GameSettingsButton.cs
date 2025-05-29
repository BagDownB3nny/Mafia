using UnityEngine;

public class GameSettingsButton : Interactable
{
    [SerializeField] public GameObject gameSettingsWindow;


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
        gameSettingsWindow.SetActive(true);
        PlayerCamera.instance.EnterCursorMode();
    }
}
