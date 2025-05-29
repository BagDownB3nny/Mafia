using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class PlayerNumberSetter : NetworkBehaviour
{

    [Header("UI")]
    [SerializeField] public RoleSettingsUI roleSettingsUI;
    [SerializeField] public TMP_Text numberText;
    [SerializeField] public GameObject leftArrowButton;
    [SerializeField] public GameObject rightArrowButton;

    int maxNumberOfPlayers = 16;

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
        int number = roleSettingsUI.expectedPlayerCount;
        numberText.text = number.ToString();
    }

    [Server]
    public void OnLeftArrowClick()
    {
        int newNumber = roleSettingsUI.expectedPlayerCount - 1;
        int currentPlayerCount = PlayerManager.instance.GetPlayerCount();
        roleSettingsUI.SetExpectedPlayerCount(newNumber);
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
        int newNumber = roleSettingsUI.expectedPlayerCount + 1;
        int currentPlayerCount = PlayerManager.instance.GetPlayerCount();
        roleSettingsUI.SetExpectedPlayerCount(newNumber);
        if (newNumber == currentPlayerCount + 1)
        {
            leftArrowButton.SetActive(true);
        } else if (newNumber == maxNumberOfPlayers) {
            rightArrowButton.SetActive(false);
        }
    }
}
