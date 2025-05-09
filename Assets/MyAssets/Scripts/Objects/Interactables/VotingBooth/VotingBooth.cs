using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class VotingBooth : Interactable
{


    // Key is player voting, value is player voted for
    public Dictionary<string, string> votes = new Dictionary<string, string>();

    // Key is player voted for, value is number of votes
    public Dictionary<string, int> votesCount = new Dictionary<string, int>();

    // Key is player voted for, value is first player to vote for them
    public Dictionary<string, string> executioners = new Dictionary<string, string>();

    // Observer
    public event Action OnVotesChanged;

    [SerializeField] private VotingSlip votingSlip;

    public static VotingBooth instance;

    public override void OnStartServer()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    public void ResetVotes()
    {
        votes.Clear();
        votesCount.Clear();
        executioners.Clear();

        InitialiseDictionaries();
    }


    [Server]
    public void InitialiseDictionaries()
    {
        List<string> playerNames = PlayerManager.instance.GetAllPlayerNames();
        foreach (string playerName in playerNames)
        {
            votesCount.Add(playerName, 0);
        }
        foreach (string playerName in playerNames)
        {
            executioners.Add(playerName, "");
        }
        OnVotesChanged?.Invoke();
    }

    [Client]
    public override void OnHover()
    {
        Highlight();
        PlayerUIManager.instance.SetInteractableText("Cast vote");
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
        votingSlip.Enable();
    }

    // Dictionary of key: player voting, value: player voted for
    [Command(requiresAuthority = false)]
    public void CmdVote(string playerVotingName, string playerVotedForName)
    {
        Debug.Log($"{playerVotingName} voted for {playerVotedForName} [Server]");
        if (votes.ContainsKey(playerVotingName))
        {
            RemoveVote(playerVotingName);
        }
        AddVote(playerVotingName, playerVotedForName);
        OnVotesChanged?.Invoke();
    }

    [Server]
    private void AddVote(string playerVotingName, string playerVotedForName)
    {
        votes.Add(playerVotingName, playerVotedForName);
        votesCount[playerVotedForName]++;
        if (executioners[playerVotedForName] == "")
        {
            executioners[playerVotedForName] = playerVotingName;
        }
    }

    [Server]
    private void RemoveVote(string playerVotingName)
    {
        string playerVotedForPreviously = votes[playerVotingName];
        votes.Remove(playerVotingName);
        votesCount[playerVotedForPreviously]--;
        if (votesCount[playerVotedForPreviously] == 0)
        {
            executioners[playerVotedForPreviously] = "";
        }
    }

    [Server]
    public Dictionary<string, int> GetVotesCount()
    {
        return votesCount;
    }

    [Server]
    public string GetVotedOutPlayer()
    {
        int maxVotes = 0;
        string playerVotedOut = "";
        foreach (KeyValuePair<string, int> vote in votesCount)
        {
            if (vote.Value > maxVotes)
            {
                maxVotes = vote.Value;
                playerVotedOut = vote.Key;
            }
            else if (vote.Value == maxVotes)
            {
                playerVotedOut = "";
            }
        }
        return playerVotedOut;
    }
}
