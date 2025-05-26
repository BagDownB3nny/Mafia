using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class VotingBooth : Interactable
{


    // Key is voter player (connId), value is suspect player voted for (connId)
    public Dictionary<int, int> votes = new();

    // Key is suspect player (connId), value is number of votes
    public Dictionary<int, int> votesCount = new();

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

    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
    }

    [Server]
    public void ResetVotes()
    {
        votes.Clear();
        votesCount.Clear();

        InitialiseDictionaries();
    }


    [Server]
    public void InitialiseDictionaries()
    {
        List<int> playerConnIds = PlayerManager.instance.GetAllPlayerConnIds();
        foreach (int connId in playerConnIds)
        {
            votesCount.Add(connId, 0);
        }
        OnVotesChanged?.Invoke();
    }

    public override string GetInteractableText()
    {
        return "[E] Cast vote";
    }


    [Client]
    public override void Interact()
    {
        votingSlip.Enable();
    }

    [Server]
    public void SubmitVote(int voterConnId, int suspectVotedForConnId)
    {
        // BUG:  An item with the same key has already been added
        if (votes.ContainsKey(voterConnId))
        {
            RemoveVote(voterConnId);
        }
        AddVote(voterConnId, suspectVotedForConnId);
        OnVotesChanged?.Invoke();
    }

    [Server]
    private void AddVote(int voterConnId, int suspectVotedForConnId)
    {
        votes.Add(voterConnId, suspectVotedForConnId);
        votesCount[suspectVotedForConnId]++;
    }

    [Server]
    private void RemoveVote(int voterConnId)
    {
        int suspectPreviouslyVotedForConnId = votes[voterConnId];
        votes.Remove(voterConnId);
        votesCount[suspectPreviouslyVotedForConnId]--;
    }

    [Server]
    public Dictionary<int, int> GetVotesCount()
    {
        return votesCount;
    }

    [Server]
    public Player GetVotedOutPlayer()
    {
        int maxVotes = 0;
        int suspectVotedOutConnId = -1;
        foreach (KeyValuePair<int, int> vote in votesCount)
        {
            if (vote.Value > maxVotes)
            {
                maxVotes = vote.Value;
                suspectVotedOutConnId = vote.Key;
            }
            else if (vote.Value == maxVotes)
            {
                suspectVotedOutConnId = -1;
            }
        }

        if (suspectVotedOutConnId == -1) return null; // No one voted out

        Player playerVotedOut = PlayerManager.instance.GetPlayerByConnId(suspectVotedOutConnId);
        return playerVotedOut;
    }
}
