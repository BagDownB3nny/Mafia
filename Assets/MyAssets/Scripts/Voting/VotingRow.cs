using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingRow : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TMP_Text playerNameText;
    public int playerConnId; // Represents the connection ID of the player
    public string playerName; // Represents the name of the player

    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
        playerName = name;
    }

    public void OnToggle()
    {
        bool isChecked = toggle.isOn;
        if (isChecked)
        {
            VotingSlip votingSlip = GetComponentInParent<VotingSlip>();
            votingSlip.SetSelectedRow(this);
        }
        else
        {
            VotingSlip votingSlip = GetComponentInParent<VotingSlip>();
            votingSlip.SetSelectedRow(null);
        }
    }

    public void Deselect()
    {
        toggle.isOn = false;
    }
}
