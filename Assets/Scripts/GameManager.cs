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
    public int TotalReputation { get; set; }
    public int TotalCoins { get; set; }

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
        RefreshGameOver();
        RefreshSceneReference();
    }

    private void RefreshSceneReference()
    {
        rewardManager = FindAnyObjectByType<RewardManager>();
        gameOverPanel = FindAnyObjectByType<GameOverUI>();
    }

    public void GameOver(DeathReason reason)
    {
        if (isGameOver)
            return;
        LastDeathReason = reason;
        rewardManager.enabled = false;
        isGameOver = true;
        OnGameOver?.Invoke();
        AudioManager.Instance.StopAllAudio();
        PauseManager.Instance.Pause(PauseReason.GameOver);
        gameOverPanel.ShowUI(reason, TotalReputation, TotalCoins);
    }

    private void RefreshGameOver()
    {
        IsPaused = false;
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

    public void ResetRun()
    {
        if (EnemyAggroManager.Instance != null)
            EnemyAggroManager.Instance.ResetAggro();
        TotalCoins = 0;
        TotalReputation = 0;
    }
}
