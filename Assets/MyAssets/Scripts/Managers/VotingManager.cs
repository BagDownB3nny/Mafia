using UnityEngine;
using Mirror;

public class VotingManager : NetworkBehaviour
{
    // Is singleton
    public static VotingManager instance;

    [SerializeField] private VotingBooth votingBooth;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("More than one instance of VotingManager found!");
            Destroy(gameObject);
            return;
        }
    }

    [ClientRpc]
    public void StartVoting()
    {
        votingBooth.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void StopVoting()
    {
        votingBooth.gameObject.SetActive(false);
    }

    [Server]
    public void StartExecution()
    {
        // Start execution
        Player votedOutPlayer = VotingBooth.instance.GetVotedOutPlayer();
        if (votedOutPlayer == null) return; // Tie breaker case, no one voted out

        ExecutePlayer(votedOutPlayer);
    }

    [Server]
    public void ExecutePlayer(Player votedOutPlayer)
    {
        votedOutPlayer.GetComponent<PlayerTeleporter>().TeleportToExecutionSpot();
        votedOutPlayer.GetComponent<PlayerMovement>().RpcLockPlayerMovement();
        votedOutPlayer.GetComponent<PlayerMovement>().RpcSetLockSigilActive(true);
    }

    [Server]
    public void StopExecution()
    {
        // End execution
        Player votedOutPlayer = VotingBooth.instance.GetVotedOutPlayer();
        if (votedOutPlayer == null) return; // Tie breaker case, no one voted out

        StopPlayerExecution(votedOutPlayer);
    }


    [Server]
    public void StopPlayerExecution(Player votedOutPlayer)
    {
        votedOutPlayer.GetComponent<PlayerMovement>().RpcUnlockPlayerMovement();
        votedOutPlayer.GetComponent<PlayerMovement>().RpcSetLockSigilActive(false);
    }
}
