using UnityEngine;
using Mirror;

public class Clock : NetworkBehaviour
{
    [SerializeField] private GameObject hourHand;
    [SerializeField] private GameObject minuteHand;

    public override void OnStartClient()
    {
        TimeManagerV2.instance.irlSecondlyClientEvent.AddListener(UpdateClock);
    }

    [Client]
    public void UpdateClock()
    {
        int minute = TimeManagerV2.instance.currentMinute;
        int hour = TimeManagerV2.instance.currentHour;
        Vector3 hourRotation = hourHand.transform.localEulerAngles;
        Vector3 minuteRotation = minuteHand.transform.localEulerAngles;
        // 0, 12 ==> 0d
        // 1, 13 ==> 30d
        hourRotation.z = hour * 30 + minute / 2;
        // 0, 60 ==> 0d
        // 15, 45 ==> 90d
        minuteRotation.z = minute * 6;

        hourHand.transform.localEulerAngles = hourRotation;
        minuteHand.transform.localEulerAngles = minuteRotation;
    }
}
