using UnityEngine;
public class InteractablePlayer : Interactable
{
    public override void OnHover()
    {
        Player currentPlayer = GetComponentInParent<Player>();
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (currentPlayer == localPlayer)
        {
            return;
        }
        string interactableText = localPlayer.GetRoleScript().RolePlayerInteractText;
        if (interactableText != null)
        {
            PlayerUIManager.instance.SetInteractableText(interactableText);
            Highlight();
        }
    }

    public override void OnUnhover()
    {
        PlayerUIManager.instance.SetInteractableText("");
        Unhighlight();
    }

    public override void Interact()
    {
        // Shouldnt do anything, since the method to invoke is in the role scripts
    }
}