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
            bool canStartGame = PlayerManager.instance.CanStartGame();
            if (canStartGame)
            {
                return "Start game";
            }
            else
            {
                return "Not enough players to start";
            }
        }
        else
        {
            return "Only lobby host can start the game";
        }
    }

    public override void Interact()
    {
        if (isServer)
        {bool canStartGame = PlayerManager.instance.CanStartGame();
            if (canStartGame)
            {
                LobbyManager.instance.StartGame();
            }
            else
            {
                return;
            }
        }
    }
}
