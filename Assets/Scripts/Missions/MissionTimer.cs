using System;
using UnityEngine;

public class MissionTimer : MonoBehaviour
{
    public static MissionTimer Instance { get; private set; }
    public float RemainingTime => remainingTime;
    public bool IsRunning => isRunning;
    public event Action OnTimerExpired;

    private float remainingTime;
    private bool isRunning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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

    public void StopTimer()
    {
        isRunning = false;
    }
}
