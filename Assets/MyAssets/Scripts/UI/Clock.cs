using Mirror;
using TMPro;
using UnityEngine;

public class Clock : NetworkBehaviour
{

    [SerializeField] private TMP_Text clockText;

    [SyncVar(hook = nameof(OnSecondsLeftChanged))]
    private int secondsLeft;


    // Only the server should update the clock
    private void Update()
    {
        if (!isServer) return;

        int newSecondsLeft = TimeManager.instance.GetSecondsLeft();
        if (newSecondsLeft != secondsLeft)
        {
            secondsLeft = newSecondsLeft;
        }
    }

    private void OnSecondsLeftChanged(int oldSecondsLeft, int newSecondsLeft)
    {
        clockText.text = newSecondsLeft.ToString();
    }
}
