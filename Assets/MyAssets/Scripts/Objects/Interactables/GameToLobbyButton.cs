using UnityEngine;
using Mirror;

public class GameToLobbyButton : Interactable
{

    [Client]
    public override void OnHover()
    {
        Highlight();
        if (isServer)
        {
            PlayerUIManager.instance.SetInteractableText("Start game");
        }
        else if (isClient)
        {
            PlayerUIManager.instance.SetInteractableText("Only lobby host can start the game");
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
            // GameManager.instance.ResetToLobby();
        }
    }
}
