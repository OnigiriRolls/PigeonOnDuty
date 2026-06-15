using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnGameOver;
    public event Action OnPaused;
    public event Action OnResumed;
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }
    public DeathReason LastDeathReason { get; private set; }

    [SerializeField] private GameObject gameOverPanel;

    private RewardManager rewardManager;

    private void Awake()
    {
        IsGameOver = false;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        rewardManager = FindAnyObjectByType<RewardManager>();
    }

    public void GameOver(DeathReason reason)
    {
        if (IsGameOver)
            return;
        LastDeathReason = reason;
        rewardManager.enabled = false;
        rewardManager.PreviousCoins = SaveManager.Instance.TotalCoins;
        IsGameOver = true;
        OnGameOver?.Invoke();
        AudioManager.Instance.StopAllAudio();
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void PauseGame()
    {
        if (IsGameOver || IsPaused)
            return;
        IsPaused = true;
        OnPaused?.Invoke();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!IsPaused)
            return;
        IsPaused = false;
        OnResumed?.Invoke();
        Time.timeScale = 1f;
    }
}
