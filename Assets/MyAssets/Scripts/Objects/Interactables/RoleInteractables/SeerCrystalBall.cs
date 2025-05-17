using Mirror;

public class SeerCrystalBall : Interactable
{
    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Seer };
    }
    public override string GetInteractableText()
    {
        return "[E] Use crystal ball";
    }

    [Client]
    public override void Interact()
    {
        // Not used right now, action has been moved to seer actions
    }
}
