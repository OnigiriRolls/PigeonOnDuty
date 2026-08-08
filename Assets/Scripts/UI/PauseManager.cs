using System.Collections.Generic;
using UnityEngine;

public enum PauseReason
{
    PauseMenu,
    MissionSelection,
    GameOver
}

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public bool IsPaused => pauseReasons.Count > 0;

    private readonly HashSet<PauseReason> pauseReasons = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool HasReason(PauseReason reason)
    {
        return pauseReasons.Contains(reason);
    }

    public void Pause(PauseReason reason)
    {
        if (pauseReasons.Add(reason))
            RefreshTimeScale();
    }

    public void Resume(PauseReason reason)
    {
        if (pauseReasons.Remove(reason))
            RefreshTimeScale();
    }

    private void RefreshTimeScale()
    {
        Time.timeScale = pauseReasons.Count > 0 ? 0f : 1f;
    }
}
