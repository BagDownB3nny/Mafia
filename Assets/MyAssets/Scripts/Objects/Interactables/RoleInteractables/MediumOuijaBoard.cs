using Mirror;
public class OuijaBoard : Interactable
{

    [SyncVar]
    public bool canBeUsed = false;
    public bool isActivated = false;


    public void OnEnable()
    {
        TimeManagerV2.instance.hourlyServerEvents[0].AddListener(EnableOuijaBoard);
        TimeManagerV2.instance.hourlyServerEvents[8].AddListener(DisableOuijaBoard);
    }

    public void OnDisable()
    {
        TimeManagerV2.instance.hourlyServerEvents[0].RemoveListener(EnableOuijaBoard);
        TimeManagerV2.instance.hourlyServerEvents[8].RemoveListener(DisableOuijaBoard);
    }

    [Server]
    public void EnableOuijaBoard()
    {
        canBeUsed = true;
    }

    [Server]
    public void DisableOuijaBoard()
    {
        canBeUsed = false;
    }

    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Medium };
    }
    public override string GetInteractableText()
    {
        if (!canBeUsed)
        {
            return Interactable.notInteractableText;
        }
        else if (isActivated)
        {
            return "[R] Stop using ouija board";
        }
        else
        {
            return "[R] Use ouija board";
        }
    }

    public override void Interact()
    {
        // Logic is in medium actions
    }
}
