using Mirror;

public class SeerCrystalBall : Interactable
{
    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Seer };
    }

    public override string GetInteractableText()
    {
        // Not used right now, action has been moved to seer actions
        return null;
    }

    [Client]
    public override void Interact()
    {
        // Not used right now, action has been moved to seer actions
    }
}
