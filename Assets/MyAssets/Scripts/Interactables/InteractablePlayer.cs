using System.Collections.Generic;
using UnityEngine;

public class InteractablePlayer : Interactable
{
    public override void OnHover()
    {
        Debug.Log("Hovering over player");
        Player localPlayer = PlayerManager.instance.localPlayer;
        string interactableText = localPlayer.GetComponent<Role>().rolePlayerInteractText;
        if (interactableText != null)
        {
            PlayerUIManager.instance.SetInteractableText(interactableText);
            Highlight();
        }
    }

    public override void OnUnhover()
    {
        Debug.Log("Unhovering");
        PlayerUIManager.instance.SetInteractableText("");
        Unhighlight();
    }

    public override void Interact()
    {
        // Shouldnt do anything, since the method to invoke is in the role scripts
    }
}