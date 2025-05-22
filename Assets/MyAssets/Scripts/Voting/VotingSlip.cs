using System;
using System.Collections.Generic;
using Mirror;
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

    private bool isVotingSlipGenerated = false;

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
            TMP_Text textComponent = child.GetComponentInChildren<TMP_Text>();
            if (textComponent.text == killedPlayer.steamUsername)
            {
                string strikedText = $"<s>{killedPlayer.steamUsername}</s>";
                textComponent.text = strikedText;
            }
        }
    }

    public void Enable()
    {
        if (!isVotingSlipGenerated)
        {
            GenerateVotingSlip();
            isVotingSlipGenerated = true;
        }
        PlayerCamera.instance.EnterCursorMode();
        VotingSlipUI.SetActive(true);
    }

    public void ExitVotingSlip()
    {
        PlayerCamera.instance.EnterFPSMode();
        VotingSlipUI.SetActive(false);
    }

    public void GenerateVotingSlip()
    {
        SyncDictionary<int, string> connIdToUsername = PlayerManager.instance.ConnIdToUsernameDict;
        foreach (KeyValuePair<int, string> kvp in connIdToUsername)
        {
            int connId = kvp.Key;
            string username = kvp.Value;
            Debug.Log($"Generating voting slip for {username} with connId {connId}");

            GameObject votingRow = Instantiate(votingPlayerRow, new Vector3(0, 0, 0), Quaternion.identity);
            votingRow.transform.SetParent(VotingTogglesContainer.transform);

            VotingRow row = votingRow.GetComponent<VotingRow>();
            row.SetPlayerName(username);
            row.playerConnId = connId;
        }
    }

    public void SubmitVote()
    {
        Toggle[] toggles = VotingTogglesContainer.GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                int playerVotedForConnId = currentlySelectedRow.playerConnId;
                Player localPlayer = PlayerManager.instance.localPlayer;
                PlayerVoter playerVoter = localPlayer.GetComponent<PlayerVoter>();
                playerVoter.CmdVote(playerVotedForConnId);
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
    }
}
