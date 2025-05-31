using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class PlayerNumberSetter : NetworkBehaviour
{

    [Header("UI")]
    [SerializeField] private RoleSettingsMenu roleSettingsMenu;
    [SerializeField] private TMP_Text numberText;
    public GameObject leftArrowButton;
    public GameObject rightArrowButton;

    readonly int maxNumberOfPlayers = 16;

    public override void OnStartClient()
    {
        // Disable arrows for clients
        if (isServer) return;

        leftArrowButton.SetActive(false);
        rightArrowButton.SetActive(false);
        SyncClientUI();
    }

    [Client]
    public void SyncClientUI()
    {
        int number = roleSettingsMenu.expectedPlayerCount;
        numberText.text = number.ToString();
    }

    [Server]
    public void OnLeftArrowClick()
    {
        int newNumber = roleSettingsMenu.expectedPlayerCount - 1;
        int currentPlayerCount = PlayerManager.instance.GetPlayerCount();
        roleSettingsMenu.SetExpectedPlayerCount(newNumber);
        if (newNumber == currentPlayerCount)
        {
            leftArrowButton.SetActive(false);
        }
        else if (newNumber == maxNumberOfPlayers - 1)
        {
            rightArrowButton.SetActive(true);
        }
    }

    [Server]
    public void OnRightArrowClick()
    {
        int newNumber = roleSettingsMenu.expectedPlayerCount + 1;
        int currentPlayerCount = PlayerManager.instance.GetPlayerCount();
        roleSettingsMenu.SetExpectedPlayerCount(newNumber);
        if (newNumber == currentPlayerCount + 1)
        {
            leftArrowButton.SetActive(true);
        } else if (newNumber == maxNumberOfPlayers) {
            rightArrowButton.SetActive(false);
        }
    }
}
