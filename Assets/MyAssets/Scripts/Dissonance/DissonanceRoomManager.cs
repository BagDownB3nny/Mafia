using System.Collections.Generic;
using Dissonance;
using UnityEngine;

public enum VoiceTriggers
{
    GlobalReceipt,
    GlobalBroadcast,
    GhostReceipt,
    GhostBroadcast,
}


public class DissonanceRoomManager : MonoBehaviour
{

    [SerializeField] List<VoiceTriggers> voiceTriggerNames = new();
    [SerializeField] DissonanceComms dissonanceComms;
    [SerializeField] List<string> voiceTriggerTokens = new();

    public static DissonanceRoomManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (voiceTriggerNames.Count != voiceTriggerTokens.Count)
        {
            Debug.LogError("Voice trigger lists are not the same length!");
            return;
        }
    }

    // Adds/removes a player to a broadcast or receipt group
    // Ghosts have a room seperate from the living, can hear the living,
    // but the living cannot hear them
    private void SetVoiceTrigger(VoiceTriggers voiceTrigger, bool isEnabled)
    {
        int index = (int)voiceTrigger;
        string voiceTriggerToken = voiceTriggerTokens[index];

        if (isEnabled)
        {
            dissonanceComms.AddToken(voiceTriggerToken);
        }
        else
        {
            dissonanceComms.RemoveToken(voiceTriggerToken);
        }
    }

    public void OnPlayerDeath()
    {
        SetVoiceTrigger(VoiceTriggers.GhostReceipt, true);
        SetVoiceTrigger(VoiceTriggers.GhostBroadcast, true);
        SetVoiceTrigger(VoiceTriggers.GlobalBroadcast, false);
    }

    public void OnPlayerAlive()
    {
        SetVoiceTrigger(VoiceTriggers.GhostReceipt, false);
        SetVoiceTrigger(VoiceTriggers.GhostBroadcast, false);
        SetVoiceTrigger(VoiceTriggers.GlobalBroadcast, true);
        SetVoiceTrigger(VoiceTriggers.GlobalReceipt, true);
    }

    public void OnMediumActivation()
    {
        SetVoiceTrigger(VoiceTriggers.GhostReceipt, true);
    }

    public void OnMediumDeactivation()
    {
        SetVoiceTrigger(VoiceTriggers.GhostReceipt, false);
    }
}
