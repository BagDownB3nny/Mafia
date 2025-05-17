using UnityEngine;
using Mirror;

public class GameStartButton : Interactable
{
    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
    }

    public override string GetInteractableText()
    {
        if (isServer)
        {
            return "Start game";
        }
        else
        {
            return "Only lobby host can start the game";
        }
    }

    public override void Interact()
    {
        if (isServer)
        {
            LobbyManager.instance.StartGame();
        }
    }
}
