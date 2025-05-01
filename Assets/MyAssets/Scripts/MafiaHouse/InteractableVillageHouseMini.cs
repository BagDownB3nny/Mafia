using UnityEngine;
using Mirror;

public class InteractableVillageHouseMini : Interactable
{

    [Header("House")]


    [SyncVar]
    public House house;

    public string PlayerName => house?.player?.steamUsername;

    [SyncVar(hook = nameof(OnIsMarkedChanged))]
    private bool isMarked = false;

    [Header("House status")]

    [SyncVar]
    public bool isOccupantDead = false;

    [SyncVar]
    public bool isHouseDestroyed = false;

    [Server]
    public void LinkHouse(House house)
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
            Debug.Log("House occupant is dead");
            return;
        }
        if (!isMarked)
        {
            MafiaHouseTable.instance.CmdSetSelectedHouseMini(this, connectionToClient);
        }
        else
        {
            MafiaHouseTable.instance.CmdSetSelectedHouseMini(null, connectionToClient);
        }
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
    public void MarkHouse()
    {
        isMarked = true;
        house.Mark();
    }

    [Server]
    public void UnmarkHouse()
    {
        isMarked = false;
        house.Unmark();
    }

    [Server]
    public void Remove()
    {
        NetworkServer.Destroy(gameObject);
    }

}
