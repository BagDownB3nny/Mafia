using TMPro;
using UnityEngine;
using Mirror;

public class TargetDummy : Interactable
{

    [SerializeField] public GameObject targetDummyVisual;
    [SerializeField] public TMP_Text linkedPlayerNameText;

    [SerializeField] public House linkedHouse;

    [SyncVar (hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    public Player linkedPlayer;


    [SyncVar (hook = nameof(OnIsActiveChanged))]
    public bool isActive = false;

    private bool isMarked = false;

    [SyncVar]
    public bool isOccupantDead = false;

    [SyncVar]
    public bool isHouseDestroyed = false;

    public void Start()
    {
        if (isServer)
        {
            PubSub.Subscribe<PlayerDeathEventHandler>(PubSubEvent.PlayerDeath, OnPlayerDeath);
            
        }
    }

    public override void OnStartClient()
    {
        targetDummyVisual.SetActive(isActive);
    }

    [Server]
    public void SetupTargetDummy(Player player, House house)
    {
        isActive = true;
        linkedPlayer = player;
        playerName = player.steamUsername;
        linkedHouse = house;
        targetDummyVisual.SetActive(true);
    }

    [Client]
    private void OnIsActiveChanged(bool oldIsActive, bool newIsActive)
    {
        targetDummyVisual.SetActive(newIsActive);
    }

    [Client]
    private void OnPlayerNameChanged(string oldName, string newName)
    {
        linkedPlayerNameText.text = newName;
    }

    [Client]
    public void SetLinkedPlayerName(string name)
    {
        playerName = name;
        linkedPlayerNameText.text = playerName;
    }

    [Server]
    public void OnPlayerDeath(Player player)
    {
        if (player == linkedPlayer)
        {
            string newText = playerName + "\n(Dead)";
            linkedPlayerNameText.text = newText;
            isOccupantDead = true;
        }
    }

    [Server]
    public void OnHouseDestroyed(House house)
    {
        if (house == linkedHouse)
        {
            string newText = playerName + "\n(House destroyed)";
            linkedPlayerNameText.text = newText;
            isHouseDestroyed = true;
        }
    }

    [Client]
    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Mafia };
    }

    [Client]
    public override string GetInteractableText()
    {
        if (isOccupantDead || isHouseDestroyed)
        {
            return notInteractableText;
        }
        if (isMarked)
        {
            return "[R] Remove curse";
        } else
        {
            return $"[R] Curse {playerName}";
        }
    }

    [Client]
    public override void Interact()
    {
        if (isOccupantDead || isHouseDestroyed)
        {
            Debug.Log("Cannot interact with this target dummy");
            return;
        }
        if (!isMarked)
        {
            TargetDummyManager.instance.CmdSetSelectedTargetdummy(this, connectionToClient);
        } else if (isMarked) {
            TargetDummyManager.instance.CmdSetSelectedTargetdummy(null, connectionToClient);
        }
        isMarked = !isMarked;
    }

    [Server]
    public void MarkHouse()
    {
        linkedHouse.Mark();
    }

    [Server]
    public void UnmarkHouse()
    {
        linkedHouse.Unmark();
    }
}
