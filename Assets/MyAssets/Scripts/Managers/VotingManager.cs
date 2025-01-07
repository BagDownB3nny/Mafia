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
        Debug.Log("Voting started");
        // Enable voting booth
        votingBooth.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void StopVoting()
    {
        Debug.Log("Voting stopped");
        votingBooth.gameObject.SetActive(false);
    }
}
