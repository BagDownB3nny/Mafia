using UnityEngine;
using Mirror;

public class InteractableVillageHouseMini : Interactable
{
    private House house;
    public string playerName => house?.player?.steamUsername;

    [SyncVar(hook = nameof(OnIsMarkedChanged))]
    private bool isMarked = false;

    public void linkHouse(House house)
    {
        this.house = house;
    }

    [Client]
    public override void OnHover()
    {
        Highlight();
        string interactableText = $"Mark {playerName}'s house";
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
        if (!isMarked)
        {
            MafiaHouseTable.instance.SetSelectedHouseMini(this);
            CmdMarkHouse();
        }
        else
        {
            MafiaHouseTable.instance.SetSelectedHouseMini(null);
            CmdUnmarkHouse();
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
}
