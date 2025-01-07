using System;

public class Timer
{
    public float RemainingSeconds { get; private set; }

    public Timer(float duration)
    {
        RemainingSeconds = duration;
    }

    public event Action OnTimerEnd;

    public void Tick(float deltaTime)
    {
        RemainingSeconds -= deltaTime;
        if (RemainingSeconds <= 0)
        {
            RemainingSeconds = 0;
            OnTimerEnd?.Invoke();
        }
    }

    public int GetSecondsLeft()
    {
        return (int)Math.Floor(RemainingSeconds);
    }
}
