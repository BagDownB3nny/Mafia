using UnityEngine;
using Mirror;

public class GameToLobbyButton : Interactable
{
    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
    }
    public override string GetInteractableText()
    {
        if (isServer)
        {
            return "End game";
        }
        else
        {
            return "Only lobby host can end the game";
        }
    }

    public override void Interact()
    {
        if (isServer)
        {
            GameEndManager.instance.EndGame();
        }
    }
}
