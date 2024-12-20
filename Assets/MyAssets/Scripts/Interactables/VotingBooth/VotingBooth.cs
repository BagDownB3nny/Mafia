using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class VotingBooth : Interactable
{

    private List<string> playersThatHaveVoted = new List<string>();
    private List<string> playersThatHaveBeenVotedFor = new List<string>();

    [SerializeField] private GameObject votingSlipCanvas;

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
        votingSlipCanvas.SetActive(true);
    }

    [Command]
    public void CmdVote(string playerVotingName, string playerVotedForName)
    {
        playersThatHaveBeenVotedFor.Add(playerVotedForName);
        playersThatHaveVoted.Add(playerVotingName);
        if (playersThatHaveVoted.Count == PlayerManager.instance.GetAllPlayers().Count)
        {
            EndVoting();
        }
    }

    [Server]
    public void EndVoting()
    {
        Debug.Log("Voting has ended");
        Debug.Log(playersThatHaveBeenVotedFor);
    }
}
