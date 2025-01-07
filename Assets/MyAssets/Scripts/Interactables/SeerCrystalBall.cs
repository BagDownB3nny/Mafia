using Mirror;

public class SeerCrystalBall : Interactable
{
    [Client]
    public override void OnHover()
    {
        Highlight();
        PlayerUIManager.instance.SetInteractableText("Look through the Seeing-Eye");
    }

    [Client]
    public override void OnUnhover()
    {
        Unhighlight();
        PlayerUIManager.instance.ClearInteractableText();
    }

    [Client]
    public override void Interact()
    {
        Player localPlayer = PlayerManager.instance.localPlayer;
        if (localPlayer.role == Roles.Seer)
        {
            Seer seerScript = localPlayer.GetComponent<Seer>();
            seerScript.LookThroughCrystalBall();
        }
    }
}
