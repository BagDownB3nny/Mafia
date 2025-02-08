using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VotingTallyBoard : MonoBehaviour
{

    [SerializeField] private VotingBooth votingBooth;
    [SerializeField] private GameObject votingTallyRowContainer;
    [SerializeField] private GameObject votingTallyRow;
    public void Start()
    {
        votingBooth.OnVotesChanged += UpdateTallyBoard;
    }

    private void UpdateTallyBoard()
    {
        // Clear the tally board
        foreach (Transform child in votingTallyRowContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("Updating Tally Board");
        Dictionary<string, int> votesCount = votingBooth.GetVotesCount();
        // for each key value pair in votesCount
        foreach (KeyValuePair<string, int> voteCount in votesCount)
        {
            GameObject votingRow = Instantiate(votingTallyRow, votingTallyRowContainer.transform);
            votingRow.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{voteCount.Key}: {voteCount.Value}";
        }
    }
}
