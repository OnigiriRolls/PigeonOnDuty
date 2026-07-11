using System;
using UnityEngine;

public class MissionTimer : MonoBehaviour
{
    public float RemainingTime => remainingTime;
    public bool IsRunning => isRunning;
    public event Action OnTimerExpired;

    private float remainingTime;
    private bool isRunning;

    private void Update()
    {
        if (!isRunning)
            return;
        remainingTime -= Time.deltaTime;
        if (remainingTime > 0f)
            return;
        remainingTime = 0f;
        isRunning = false;
        OnTimerExpired?.Invoke();
    }

    public void StartTimer(float duration, float multiplier = 1f)
    {
        remainingTime = duration * multiplier;
        isRunning = true;
    }

    public void AddTime(float seconds)
    {
        remainingTime += seconds;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
