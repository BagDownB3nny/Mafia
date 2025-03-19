using UnityEngine;
using System;
using System.Collections.Generic;

public enum PubSubEvent
{
    PlayerDeath,
    HouseDestroyed,
    // PlayerHealthChanged,
    // EnemyKilled
}

// Define specific delegates for each event type
public delegate void PlayerDeathEventHandler(Player killedPlayer);
public delegate void HouseDestroyedEventHandler(House destroyedHouse);
// public delegate void PlayerHealthChangedEventHandler(int currentHealth, int maxHealth);
// public delegate void EnemyKilledEventHandler(string enemyType, int points);


// This pubsub class is only used by the server, not client
public static class PubSub
{
    // Dictionary to store subscribers for each event type
    private static Dictionary<PubSubEvent, Delegate> _subscribers = new Dictionary<PubSubEvent, Delegate>();

    // Subscribe to an event with a strongly-typed handler
    public static void Subscribe<T>(PubSubEvent eventType, T handler) where T : Delegate
    {
        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = Delegate.Combine(_subscribers[eventType], handler);
        }
        else
        {
            _subscribers[eventType] = handler;
        }
    }

    // Unsubscribe from an event
    public static void Unsubscribe<T>(PubSubEvent eventType, T handler) where T : Delegate
    {
        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = Delegate.Remove(_subscribers[eventType], handler);

            // Clean up if no subscribers remain
            if (_subscribers[eventType] == null)
            {
                _subscribers.Remove(eventType);
            }
        }
    }

    // Publish an event with strongly-typed data
    public static void Publish<T>(PubSubEvent eventType, T data)
    {
        if (_subscribers.ContainsKey(eventType))
        {
            var handler = _subscribers[eventType] as Action<T>;
            handler?.Invoke(data);
        }
    }
}