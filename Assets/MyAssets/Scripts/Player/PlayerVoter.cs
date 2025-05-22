using Mirror;
using UnityEngine;

public class PlayerVoter : NetworkBehaviour
{

    [Command]
    public void CmdVote(int playerVotedForConnId)
    {
        int playerVotingConnId = connectionToClient.connectionId;
        VotingBooth.instance.SubmitVote(playerVotingConnId, playerVotedForConnId);
    }
}
