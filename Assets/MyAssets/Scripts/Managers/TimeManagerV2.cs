using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;

public class TimeManagerV2 : NetworkBehaviour
{
    [Header("ServerEvents")]

    // These events are invoked by the server
    public UnityEvent[] hourlyServerEvents = new UnityEvent[24];
    public UnityEvent irlSecondlyServerEvent = new();

    [Header("ClientEvents")]

    // These events are invoked by the client
    // Client events should only affect the appearance of the game on the client (eg. clock, lights)
    public UnityEvent[] hourlyClientEvents = new UnityEvent[24];
    public UnityEvent irlSecondlyClientEvent = new();



    [Header("Internal params")]

    // The syncvar hooks will allow for clients to update their game based on time changes

    [SyncVar(hook = nameof(TriggerClientHourlyEvent))]
    public int currentHour = 0;

    [SyncVar(hook = nameof(TriggerClientMinutelyEvent))]
    public int currentMinute = 0;

    public static TimeManagerV2 instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnStartServer()
    {
        hourlyServerEvents[0].AddListener(TwelveAmEvent);
        hourlyServerEvents[8].AddListener(EightAmEvent);
        hourlyServerEvents[18].AddListener(SixPmEvent);
        hourlyServerEvents[22].AddListener(TenPmEvent);
        hourlyServerEvents[23].AddListener(ElevenPmEvent);
    }

    [Server]
    public void StartGame()
    {
        currentHour = 19;
        currentMinute = 0;
        TriggerHourlyEvent();
        InvokeRepeating(nameof(TickMinuteHand), 1f, 1f);
    }

    [Server]
    private void TickMinuteHand()
    {
        float irlSecondsPerGameHour = GameSettingsManager.instance.irlSecondsPerGameHour;
        int newMinute = currentMinute + Convert.ToInt32(Math.Floor(60 / irlSecondsPerGameHour));
        int newHour = currentHour;
        if (newMinute >= 60)
        {
            newMinute = 0;
            newHour++;
            if (newHour >= 24)
            {
                newHour = 0;
            }
        }
        SetNewTime(newHour, newMinute);
    }

    [Server]
    private void SetNewTime(int newHour, int newMinute)
    {
        bool triggerHourlyEvent = newHour != currentHour;
        currentHour = newHour;
        currentMinute = newMinute;

        irlSecondlyServerEvent.Invoke();
        if (triggerHourlyEvent)
        {
            TriggerHourlyEvent();
        }
    }

    private void TriggerClientHourlyEvent(int oldHour, int newHour)
    {
        if (isClient)
        {
            // Debug.Log($"Triggering client hourly event for hour {newHour}");
            hourlyClientEvents[newHour].Invoke();
        }
    }

    private void TriggerClientMinutelyEvent(int oldMinute, int newMinute)
    {
        if (isClient)
        {
            irlSecondlyClientEvent.Invoke();
        }
    }

    [Server]
    private void TriggerHourlyEvent()
    {
        hourlyServerEvents[currentHour]?.Invoke();
    }

    [Server]
    private void TwelveAmEvent()
    {
        HouseManager.instance.UnhighlightHousesForOwners();

        // Give mafia members guns
        PlayerManager.instance.GetMafiaPlayers().ForEach(player =>
        {
            player.house.UnlockTrapDoor();
        });

        PlayerManager.instance.RemoveAllNametagsForNonMafia();
    }

    [Server]
    private void EightAmEvent()
    {
        PlayerManager.instance.AddAllNametagsForNonMafia();
        // Take away mafia members' guns
        List<Player> mafiaPlayers = PlayerManager.instance.GetMafiaPlayers();
        if (mafiaPlayers.Count == 0)
        {
            Debug.Log("No mafia players found");
        }
        else
        {
            mafiaPlayers.ForEach(player =>
            {
                Role roleScript = player.GetRoleScript();
                if (roleScript is Mafia mafiaRole)
                {
                    mafiaRole.ServerDisableMafiaInventorySlots();
                }
            });
        }

        // Activate voting booth
        VotingManager.instance.StartVoting();
        VotingBooth.instance.ResetVotes();

        // Reset all sigils
        SigilsManager.instance.ResetAllSigils();

        // Clear mafia target selection
        TargetDummyManager.instance.ClearSelection();

        // Turn off all lights
        LightManager.instance.TurnOffAllLights();

        // Add all nametags
    }

    [Server]
    private void SixPmEvent()
    {
        // Deactivate voting booth
        VotingManager.instance.StopVoting();
        VotingManager.instance.StartExecution();
        LightManager.instance.TurnOnAllLights();
    }


    [Server]
    private void TenPmEvent()
    {
        VotingManager.instance.StopExecution();
        HouseManager.instance.HighlightHousesForOwners();
    }

    [Server]
    private void ElevenPmEvent()
    {
        // End execution
        // string votedOutPlayerName = VotingBooth.instance.GetVotedOutPlayer();

        // if (votedOutPlayerName != "")
        // {
        //     Player votedOutPlayer = PlayerManager.instance.GetPlayerByName(votedOutPlayerName);
        //     votedOutPlayer.GetComponent<PlayerMovement>().UnlockPlayerMovement();
        // }
    }

    public bool IsBetweenMidnightAndMorning()
    {
        return currentHour >= 0 && currentHour < 8;
    }
}
