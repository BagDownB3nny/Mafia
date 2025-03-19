using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingSlip : MonoBehaviour
{
    [SerializeField] private GameObject VotingTogglesContainer;
    [SerializeField] private GameObject VotingSlipUI;
    private List<String> playerNames;
    [SerializeField] private GameObject votingPlayerRow;
    [SerializeField] private VotingBooth votingBooth;
    private VotingRow currentlySelectedRow;

    public static VotingSlip instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            return;
        }
    }

    public void Start()
    {
        PubSub.Subscribe<PlayerDeathEventHandler>(PubSubEvent.PlayerDeath, OnPlayerDeath);
    }

    private void OnPlayerDeath(Player killedPlayer)
    {
        foreach (Transform child in VotingTogglesContainer.transform)
        {
            Debug.Log(killedPlayer.steamUsername);
            TMP_Text textComponent = child.GetComponentInChildren<TMP_Text>();
            Debug.Log(textComponent.text);
            if (textComponent.text == killedPlayer.steamUsername)
            {
                string strikedText = $"<s>{killedPlayer.steamUsername}</s>";
                textComponent.text = strikedText;
            }
        }
    }

    public void Enable()
    {
        if (playerNames == null)
        {
            GenerateVotingSlip();
        }
        PlayerCamera.instance.EnterCursorMode();
        VotingSlipUI.SetActive(true);
    }

    public void ExitVotingSlip()
    {
        PlayerCamera.instance.EnterFPSMode();
        gameObject.SetActive(false);
    }

    public void GenerateVotingSlip()
    {
        playerNames = PlayerManager.instance.GetAllPlayerNames();
        for (int i = 0; i < playerNames.Count; i++)
        {
            GameObject votingRow = Instantiate(votingPlayerRow, new Vector3(0, 0, 0), Quaternion.identity);
            votingRow.transform.SetParent(VotingTogglesContainer.transform);
            votingRow.GetComponentInChildren<TMP_Text>().text = playerNames[i];
        }
    }

    public void SubmitVote()
    {
        Toggle[] toggles = VotingTogglesContainer.GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                string playerVotedForName = toggle.GetComponentInChildren<Text>().text;
                string playerVotingName = PlayerManager.instance.GetLocalPlayerName();
                Debug.Log($"{playerVotingName} voted for {playerVotedForName}");
                votingBooth.CmdVote(playerVotingName, playerVotedForName);
                ExitVotingSlip();
            }
        }
    }

    public void SetSelectedRow(VotingRow row)
    {
        if (currentlySelectedRow != null)
        {
            currentlySelectedRow.Deselect();
        }
        currentlySelectedRow = row;
        Debug.Log("Setting selected row");
    }
}
