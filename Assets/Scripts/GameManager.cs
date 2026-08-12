using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action OnGameOver;
    public event Action OnPaused;
    public event Action OnResumed;
    public bool IsPaused { get; private set; }
    public DeathReason LastDeathReason { get; private set; }

    private GameOverUI gameOverPanel;
    private RewardManager rewardManager;
    private bool isGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        isGameOver = false;
        Time.timeScale = 1f;
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        RefreshSceneReference();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSceneReference();
    }

    private void RefreshSceneReference()
    {
        rewardManager = FindAnyObjectByType<RewardManager>();
        gameOverPanel = FindAnyObjectByType<GameOverUI>();
        RefreshGameOver();
    }

    public void GameOver(DeathReason reason)
    {
        if (isGameOver)
            return;
        LastDeathReason = reason;
        rewardManager.enabled = false;
        rewardManager.PreviousCoins = SaveManager.Instance.TotalCoins;
        isGameOver = true;
        OnGameOver?.Invoke();
        AudioManager.Instance.StopAllAudio();
        PauseManager.Instance.Pause(PauseReason.GameOver);
        gameOverPanel.ShowUI(reason, rewardManager.Reputation, rewardManager.TotalCoins, rewardManager.PreviousCoins);
    }

    private void RefreshGameOver()
    {
        isGameOver = false;
    }

    public void PauseGame()
    {
        if (isGameOver || IsPaused)
            return;
        IsPaused = true;
        OnPaused?.Invoke();
        PauseManager.Instance.Pause(PauseReason.PauseMenu);
    }

    public void ResumeGame()
    {
        if (!IsPaused)
            return;
        IsPaused = false;
        OnResumed?.Invoke();
        PauseManager.Instance.Resume(PauseReason.PauseMenu);
    }
}
