using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameSettingsManager : NetworkBehaviour
{

    [Header("Game Settings")]

    [SyncVar]
    public float irlSecondsPerGameHour;

    public static GameSettingsManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            irlSecondsPerGameHour = PlayerPrefs.GetInt("irlSecondsPerGameHour", 6);
            Debug.Log($"GameSettingsManager Awake: irlSecondsPerGameHour = {irlSecondsPerGameHour}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetIrlsPerGameHour(float irlsPerGameHour)
    {
        irlSecondsPerGameHour = irlsPerGameHour;
        PlayerPrefs.SetInt("irlSecondsPerGameHour", (int)irlSecondsPerGameHour);        
    }
}
