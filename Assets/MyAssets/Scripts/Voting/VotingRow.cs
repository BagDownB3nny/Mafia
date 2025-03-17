using UnityEngine;
using UnityEngine.UI;

public class VotingRow : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

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
