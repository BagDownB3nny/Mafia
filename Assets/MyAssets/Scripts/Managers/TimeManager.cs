using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class TimeManager : NetworkBehaviour
{

    [Header("Duration")]
    [SerializeField] private float dayDuration = 60;
    [SerializeField] private float nightDuration = 60;

    public UnityEvent OnDayEndEvent;
    public UnityEvent OnNightStartEvent;
    public UnityEvent OnNightEndEvent;
    public UnityEvent OnDayStartEvent;

    private Timer clock;
    public int dayNumber { get; private set; } = 1;
    public bool isCurrentlyDay { get; private set; } = true;

    public static TimeManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // TODO: Start day should be called after all players have loaded in
            OnDayStart();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        clock.Tick(Time.deltaTime);
    }

    public int GetSecondsLeft()
    {
        return clock.GetSecondsLeft();
    }

    [Server]
    private void OnDayEnd()
    {
        Debug.Log($"Day {dayNumber} ended");
        OnNightStart();
    }

    [Server]
    private void OnNightStart()
    {
        Debug.Log($"Night {dayNumber} started");
        // Close doors
        HouseManager.instance.SetActiveAllDoors();
        // Set moon
        // Set clock
        clock = new Timer(nightDuration);
        clock.OnTimerEnd += OnNightEnd;
        // Teleport players back to home
        PlayerManager.instance.TeleportAllPlayersBackToSpawn();

        // TODO: Only give guns to mafia
        List<Player> players = PlayerManager.instance.GetAllPlayers();
        foreach (Player player in players)
        {
            player.EquipGun();
        }
    }

    [Server]
    private void OnNightEnd()
    {
        Debug.Log($"Night {dayNumber} ended");
        dayNumber += 1;


        List<Player> players = PlayerManager.instance.GetAllPlayers();
        foreach (Player player in players)
        {
            player.UnequipGun();
        }
        // Bring killers back to house
        // Open doors
        HouseManager.instance.SetInactiveAllDoors();
        // Remove moon
        OnDayStart();
    }

    [Server]
    private void OnDayStart()
    {
        dayNumber += 1;
        Debug.Log($"Day {dayNumber} started");
        // Set clock
        // Set sun
        clock = new Timer(dayDuration);
        clock.OnTimerEnd += OnDayEnd;
    }
}
