using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VotingSlip : MonoBehaviour
{
    [SerializeField] private GameObject VotingTogglesContainer;
    private List<String> playerNames;
    private List<String> deadPlayerNames;
    [SerializeField] private GameObject votingPlayerRow;
    [SerializeField] private VotingBooth votingBooth;


    public void ExitVotingSlip()
    {
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        if (playerNames == null)
        {
            GenerateVotingSlip();
        }
    }

    private void GenerateVotingSlip()
    {
        playerNames = PlayerManager.instance.GetAllPlayerNames();
        for (int i = 0; i < playerNames.Count; i++)
        {
            GameObject votingRow = Instantiate(votingPlayerRow, new Vector3(0, 0, 0), Quaternion.identity);
            votingRow.transform.SetParent(VotingTogglesContainer.transform);
            votingRow.GetComponentInChildren<Text>().text = playerNames[i];
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
}
