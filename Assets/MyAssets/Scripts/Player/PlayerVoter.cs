using Mirror;
using UnityEngine;

public class PlayerVoter : NetworkBehaviour
{

    [Command]
    public void CmdVote(int suspectVotedForConnId)
    {
        int voterConnId = connectionToClient.connectionId;
        VotingBooth.instance.SubmitVote(voterConnId, suspectVotedForConnId);
    }
}
