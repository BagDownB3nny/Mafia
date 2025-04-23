using UnityEngine;
using Mirror;

public class InteractableVillageHouseMini : Interactable
{
    public House house;
    public string PlayerName => house?.player?.steamUsername;

    [SyncVar(hook = nameof(OnIsMarkedChanged))]
    private bool isMarked = false;

    [SyncVar]
    public bool isOccupantDead = false;

    [SyncVar]
    public bool isHouseDestroyed = false;

    public void linkHouse(House house)
    {
        this.house = house;
    }

    [Client]
    public override void OnHover()
    {
        Highlight();
        if (isOccupantDead)
        {
            PlayerUIManager.instance.SetInteractableText($"{PlayerName} is dead");
            return;
        }

        if (isHouseDestroyed)
        {
            PlayerUIManager.instance.SetInteractableText($"{PlayerName}'s house is destroyed. {PlayerName} must be hiding somewhere...");
            return;
        }
        string interactableText = $"Mark {PlayerName}'s house";
        PlayerUIManager.instance.SetInteractableText(interactableText);
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
        if (isOccupantDead)
        {
            return;
        }
        if (!isMarked)
        {
            MafiaHouseTable.instance.SetSelectedHouseMini(this);
        }
        else
        {
            MafiaHouseTable.instance.SetSelectedHouseMini(null);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdMarkHouse()
    {
        isMarked = true;
        house.DeactivateProtection();
    }

    [Command(requiresAuthority = false)]
    public void CmdUnmarkHouse()
    {
        isMarked = false;
        house.ActivateProtection();
    }

    [Client]
    private void OnIsMarkedChanged(bool oldMarked, bool newMarked)
    {
        Debug.Log($"Client marked: {newMarked}");
        // TODO:
        // Enable visual effect for marked house
        // Disable visual effect for unmarked house
    }

    [Server]
    public void Remove()
    {
        NetworkServer.Destroy(gameObject);
    }

}
