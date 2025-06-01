using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class TimeSettingsMenu : NetworkBehaviour
{
    [Header("Time Settings")]

    private readonly List<int> timeOptions = new() { 3, 4, 5, 6, 7, 8, 9, 10 };
    private int currentTimeIndex;

    [SyncVar (hook = nameof(OnIrlSecondsPerGameHourChanged))]
    public int irlSecondsPerGameHour;

    [Header("UI Elements")]
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    [SerializeField] private TMPro.TextMeshProUGUI timeText;

    public override void OnStartServer()
    {
        currentTimeIndex = PlayerPrefs.GetInt("timeIndex", 3);
        irlSecondsPerGameHour = timeOptions[currentTimeIndex];
        timeText.text = irlSecondsPerGameHour.ToString();
    }

    public override void OnStartClient()
    {
        if (isServer) return;

        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        timeText.text = irlSecondsPerGameHour.ToString();
    }

    
    [Client]
    public void OnIrlSecondsPerGameHourChanged(int oldNumber, int newNumber)
    {
        timeText.text = newNumber.ToString();
    }

    public void OnLeftArrowTimeClick()
    {
        currentTimeIndex--;
        irlSecondsPerGameHour = timeOptions[currentTimeIndex];
        timeText.text = irlSecondsPerGameHour.ToString();

        if (currentTimeIndex == 0)
        {
            leftArrow.SetActive(false);
        }
        else if (currentTimeIndex == timeOptions.Count - 2)
        {
            rightArrow.SetActive(true);
        }
    }

    public void OnRightArrowTimeClick()
    {
        currentTimeIndex++;
        irlSecondsPerGameHour = timeOptions[currentTimeIndex];
        timeText.text = irlSecondsPerGameHour.ToString();

        if (currentTimeIndex == timeOptions.Count - 1)
        {
            rightArrow.SetActive(false);
        }
        else if (currentTimeIndex == 1)
        {
            leftArrow.SetActive(true);
        }
    }

    public void SaveTimeSetting()
    {
        PlayerPrefs.SetInt("timeIndex", currentTimeIndex);
        PlayerPrefs.SetInt("irlSecondsPerGameHour", irlSecondsPerGameHour);
        PlayerPrefs.Save();
    }
}
