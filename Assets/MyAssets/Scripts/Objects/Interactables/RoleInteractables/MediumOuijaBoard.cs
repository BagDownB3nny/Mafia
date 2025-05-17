using Mirror;
public class OuijaBoard : Interactable
{
    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Medium };
    }
    public override string GetInteractableText()
    {
        return "[E] Use ouija board";
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
