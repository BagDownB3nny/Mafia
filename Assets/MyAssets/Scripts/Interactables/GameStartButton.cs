using UnityEngine;
using Mirror;

public class GameStartButton : Interactable
{

    [Client]
    public override void OnHover()
    {
        Highlight();
        if (isClient)
        {
            PlayerUIManager.instance.SetInteractableText("Only lobby host can start the game");
        }
        else if (this.isServer)
        {
            PlayerUIManager.instance.SetInteractableText("Start game");
        }
    }

    [Client]
    public override void OnUnhover()
    {
        Unhighlight();
        PlayerUIManager.instance.ClearInteractableText();
    }

    public override void Interact()
    {
        if (isServer)
        {
            LobbyManager.instance.StartGame();
        }
    }
}
