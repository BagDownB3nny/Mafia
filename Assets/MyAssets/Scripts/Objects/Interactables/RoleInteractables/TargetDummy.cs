using TMPro;
using UnityEngine;

public class TargetDummy : Interactable
{

    [SerializeField] private TMP_Text linkedPlayerNameText;
    private string playerName;
    public Player linkedPlayer;
    public bool isActive = false;

    private bool isMarked = false;

    public void Start()
    {
        if (isServer)
        {
            PubSub.Subscribe<PlayerDeathEventHandler>(PubSubEvent.PlayerDeath, OnPlayerDeath);
        }
        // Since network identity causes the gameobject to be active
        SetActive(isActive);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);

        // This is so that 
        if (active)
        {
            isActive = true;
        }
    }

    public void SetLinkedPlayer(Player player)
    {
        linkedPlayer = player;
    }

    public void SetLinkedPlayerName(string name)
    {
        playerName = name;
        linkedPlayerNameText.text = playerName;
    }

    public void OnPlayerDeath(Player player)
    {
        if (player == linkedPlayer)
        {
            string newText = playerName + "\n(Dead)";
            linkedPlayerNameText.text = newText;
        }
    }

    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Mafia };
    }

    public override string GetInteractableText()
    {
        if (isMarked)
        {
            return "Remove curse";
        } else
        {
            return $"Curse {playerName}";
        }
    }

    public override void Interact()
    {
        if (!isMarked)
        {
            Whiteboard.instance.SetNewMarkedPlayer(playerName);
        } else if (isMarked) {
            Whiteboard.instance.ClearWhiteboard();
        }
        isMarked = !isMarked;
    }
}
