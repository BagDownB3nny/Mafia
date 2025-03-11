using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;

public class TimeManagerV2 : NetworkBehaviour
{

    [Header("Time settings")]
    [SerializeField] public float irlSecondsPerGameHour = 3;

    [Header("Events")]
    public UnityEvent[] hourlyEvents = new UnityEvent[24];
    public UnityEvent irlSecondlyEvent = new UnityEvent();

    [Header("Internal params")]
    public int currentHour { get; private set; } = 0;
    public int currentMinute { get; private set; } = 0;

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
        hourlyEvents[0].AddListener(TwelveAmEvent);
        hourlyEvents[8].AddListener(EightAmEvent);
        hourlyEvents[18].AddListener(SixPmEvent);
        hourlyEvents[23].AddListener(ElevenPmEvent);
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
        currentMinute += Convert.ToInt32(Math.Floor(60 / irlSecondsPerGameHour));
        irlSecondlyEvent.Invoke();

        if (currentMinute >= 60)
        {
            currentMinute = 0;
            currentHour++;
            TriggerHourlyEvent();
        }
        Debug.Log($"Time: {currentHour}:{currentMinute}");
    }

    [Server]
    private void TriggerHourlyEvent()
    {
        if (currentHour >= 24)
        {
            currentHour = 0;
        }
        if (hourlyEvents[currentHour] != null)
        {
            hourlyEvents[currentHour].Invoke();
        }
    }

    [Server]
    private void TwelveAmEvent()
    {
        // TODO: Unlock and open all mafia hatches

        // Give mafia members guns
        PlayerManager.instance.GetMafiaPlayers().ForEach(player =>
        {
            player.EquipGun();
        });
    }

    [Server]
    private void EightAmEvent()
    {
        // TODO: Lock all mafia hatches

        // Take away mafia members' guns
        PlayerManager.instance.GetMafiaPlayers().ForEach(player =>
        {
            player.UnequipGun();
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
