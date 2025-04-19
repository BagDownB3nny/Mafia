using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;

public class TimeManagerV2 : NetworkBehaviour
{

    [Header("Time settings")]
    [SerializeField] public float irlSecondsPerGameHour = 1f;

    [Header("ServerEvents")]

    // These events are invoked by the server
    public UnityEvent[] hourlyServerEvents = new UnityEvent[24];
    public UnityEvent irlSecondlyServerEvent = new UnityEvent();

    [Header("ClientEvents")]

    // These events are invoked by the client
    // Client events should only affect the appearance of the game on the client (eg. clock, lights)
    public UnityEvent[] hourlyClientEvents = new UnityEvent[24];
    public UnityEvent irlSecondlyClientEvent = new UnityEvent();



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
        hourlyServerEvents[23].AddListener(ElevenPmEvent);
    }

    [Server]
    public void StartGame()
    {
        currentHour = 19;
        currentMinute = 0;
        TriggerHourlyEvent();
        InvokeRepeating("TickMinuteHand", 1f, 1f);
    }

    [Server]
    private void TickMinuteHand()
    {
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
        Debug.Log($"Time: {currentHour}:{currentMinute}");

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
            Debug.Log($"Triggering client hourly event {newHour}");
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
        if (hourlyServerEvents[currentHour] != null)
        {
            hourlyServerEvents[currentHour].Invoke();
        }
    }

    [Server]
    private void TwelveAmEvent()
    {
        // Give mafia members guns
        PlayerManager.instance.GetMafiaPlayers().ForEach(player =>
        {
            player.EquipGun();
            player.house.UnlockTrapDoor();
        });
    }

    [Server]
    private void EightAmEvent()
    {
        // Take away mafia members' guns
        PlayerManager.instance.GetMafiaPlayers().ForEach(player =>
        {
            player.UnequipGun();
            player.house.LockTrapDoor();
        });

        // Activate voting booth
        VotingManager.instance.StartVoting();

        // Reset all sigils
        SigilsManager.instance.ResetAllSigils();

        // Turn off all lights
        LightManager.instance.TurnOffAllLights();
    }

    [Server]
    private void SixPmEvent()
    {
        // Deactivate voting booth
        VotingManager.instance.StopVoting();

        // Start execution
        string votedOutPlayerName = VotingBooth.instance.GetVotedOutPlayer();

        if (votedOutPlayerName != "")
        {
            Player votedOutPlayer = PlayerManager.instance.GetPlayerByName(votedOutPlayerName);
            votedOutPlayer.GetComponent<PlayerTeleporter>().TeleportToExecutionSpot();

            // TODO: Restrict that player's movement from 6pm till 11pm (gives chance for them to be spared)
            votedOutPlayer.GetComponent<PlayerMovement>().LockPlayerMovement();
        }

        // Turn on all lights
        LightManager.instance.TurnOnAllLights();
    }

    [Server]
    private void ElevenPmEvent()
    {
        // End execution
        string votedOutPlayerName = VotingBooth.instance.GetVotedOutPlayer();

        if (votedOutPlayerName != "")
        {
            Player votedOutPlayer = PlayerManager.instance.GetPlayerByName(votedOutPlayerName);
            votedOutPlayer.GetComponent<PlayerMovement>().UnlockPlayerMovement();
        }
    }
}
