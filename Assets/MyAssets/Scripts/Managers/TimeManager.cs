using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class TimeManager : NetworkBehaviour
{

    [Header("Duration")]
    [SerializeField] private float dayDuration = 20;
    [SerializeField] private float nightDuration = 60;

    public UnityEvent OnDayEndEvent;
    public UnityEvent OnNightStartEvent;
    public UnityEvent OnNightEndEvent;
    public UnityEvent OnDayStartEvent;

    private Timer clock;
    public int dayNumber { get; private set; } = 0;
    public bool isCurrentlyDay { get; private set; } = true;

    public static TimeManager instance;

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
    private void Update()
    {
        if (clock != null)
        {
            clock.Tick(Time.deltaTime);
        }
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
        // Close doors and protect all houses
        HouseManager.instance.CloseAllDoors();
        HouseManager.instance.ProtectAllHouses();
        // Unprotect the house marked for death
        DeathSigil.ActivateAtNight();
        ProtectionSigil.ActivateAtNight();
        // Set moon
        // Set clock
        clock = new Timer(nightDuration);
        clock.OnTimerEnd += OnNightEnd;
        // Teleport players back to home
        PlayerManager.instance.TeleportAllPlayersBackToNightSpawn();

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

        List<Player> players = PlayerManager.instance.GetAllPlayers();
        foreach (Player player in players)
        {
            player.UnequipGun();
        }
        // Bring everyone back to house
        PlayerManager.instance.TeleportAllPlayersBackToSpawn();
        // Open doors
        // Remove moon
        OnDayStart();
    }

    public void StartFirstDay()
    {
        OnDayStart();
    }

    [Server]
    private void OnDayStart()
    {
        // Reset all sigils
        DeathSigil.ResetAllDeathSigils();

        dayNumber += 1;
        Debug.Log($"Day {dayNumber} started");
        // Set clock
        // Set sun
        clock = new Timer(dayDuration);
        clock.OnTimerEnd += OnDayEnd;

        // Close all doors
        HouseManager.instance.CloseAllDoors();
    }
}
