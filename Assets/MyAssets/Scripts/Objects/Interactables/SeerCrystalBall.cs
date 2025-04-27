using Mirror;

public class SeerCrystalBall : Interactable
{
    [Client]
    public override void OnHover()
    {
        Highlight();
        PlayerUIManager.instance.SetInteractableText("[E] Use crystal ball");
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
        if (localPlayer.role == RoleName.Seer)
        {
            Seer seerScript = localPlayer.GetComponentInChildren<Seer>();
            seerScript.LookThroughCrystalBall();
        }
    }
}
