using Mirror;
public class OuijaBoard : Interactable
{
    [Client]
    public override void OnHover()
    {
        Highlight();
        PlayerUIManager.instance.SetInteractableText("[E] Use ouija board to commune with the dead");
    }
    public override void OnUnhover()
    {
        Unhighlight();
        PlayerUIManager.instance.ClearInteractableText();
    }
    public override void Interact()
    {
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (localPlayer.role == RoleName.Medium)
        {
            Medium mediumScript = localPlayer.GetComponentInChildren<Medium>();
            mediumScript.ActivateMediumAbility();
        }
    }
}
