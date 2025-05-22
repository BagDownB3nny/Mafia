using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class VotingBooth : Interactable
{


    // Key is player voting, (connId) value is player voted for (connId)
    public Dictionary<int, int> votes = new Dictionary<int, int>();

    // Key is player voted for (connId), value is number of votes
    public Dictionary<int, int> votesCount = new Dictionary<int, int>();

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
    public void SubmitVote(int playerVotingConnId, int playerVotingForConnId)
    {
        if (votes.ContainsKey(playerVotingConnId))
        {
            RemoveVote(playerVotingConnId);
        }
        AddVote(playerVotingConnId, playerVotingForConnId);
        OnVotesChanged?.Invoke();
    }

    [Server]
    private void AddVote(int playerVotingConnId, int playerVotedForConnId)
    {
        votes.Add(playerVotingConnId, playerVotedForConnId);
        votesCount[playerVotedForConnId]++;
    }

    [Server]
    private void RemoveVote(int playerVotingConnId)
    {
        int playerVotedForPreviously = votes[playerVotingConnId];
        votes.Remove(playerVotedForPreviously);
        votesCount[playerVotedForPreviously]--;
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
        int playerVotedOutConnId = -1;
        foreach (KeyValuePair<int, int> vote in votesCount)
        {
            if (vote.Value > maxVotes)
            {
                maxVotes = vote.Value;
                playerVotedOutConnId = vote.Key;
            }
            else if (vote.Value == maxVotes)
            {
                playerVotedOutConnId = -1;
            }
        }

        if (playerVotedOutConnId == -1) return null; // No one voted out

        Player playerVotedOut = PlayerManager.instance.GetPlayerByConnId(playerVotedOutConnId);
        return playerVotedOut;
    }
}
