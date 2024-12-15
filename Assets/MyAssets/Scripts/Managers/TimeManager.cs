using System;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
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

    private void OnDayEnd()
    {
        Debug.Log("Day ended");
        OnNightStart();
    }

    private void OnNightStart()
    {
        Debug.Log("Night started");
        // Close doors
        // Set moon
        // Set clock
        clock = new Timer(nightDuration);
        clock.OnTimerEnd += OnNightEnd;
        // Teleport players back to home
        PlayerManager.instance.TeleportAllPlayersBackToSpawn();
    }

    private void OnNightEnd()
    {
        Debug.Log("Night ended");
        // Bring killers back to house
        // Open doors
        // Remove moon
        OnDayStart();
    }

    private void OnDayStart()
    {
        Debug.Log("Day started");
        // Set clock
        // Set sun
        clock = new Timer(dayDuration);
        clock.OnTimerEnd += OnDayEnd;
    }
}
