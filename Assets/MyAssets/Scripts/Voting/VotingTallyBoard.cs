using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;

public class VotingTallyBoard : NetworkBehaviour
{

    [SerializeField] private VotingBooth votingBooth;
    [SerializeField] private GameObject votingTallyRowContainer;
    [SerializeField] private GameObject votingTallyRow;
    public override void OnStartServer()
    {
        votingBooth.OnVotesChanged += UpdateTallyBoard;
    }

    [Server]

    private void UpdateTallyBoard()
    {
        Dictionary<string, int> votesCount = votingBooth.GetVotesCount();
        string votesCountJson = JsonConvert.SerializeObject(votesCount);
        RpcUpdateTallyBoard(votesCountJson);
    }

    [ClientRpc]
    private void RpcUpdateTallyBoard(string votesCountJson)
    {
        Dictionary<string, int> votesCount = JsonConvert.DeserializeObject<Dictionary<string, int>>(votesCountJson);

        // Clear the tally board
        foreach (Transform child in votingTallyRowContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // for each key value pair in votesCount
        foreach (KeyValuePair<string, int> voteCount in votesCount)
        {
            GameObject votingRow = Instantiate(votingTallyRow, votingTallyRowContainer.transform);
            votingRow.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{voteCount.Key}: {voteCount.Value}";
        }
    }
}
