using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnGameOver;
    public event Action OnPaused;
    public event Action OnResumed;
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    [SerializeField] private GameObject gameOverPanel;

    private ScoreManager scoreManager;

    private void Awake()
    {
        IsGameOver = false;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    public void GameOver()
    {
        if (IsGameOver)
            return;

        scoreManager.enabled = false;
        scoreManager.FinishRun();
        CurrencyManager.Instance.AddCoins(scoreManager.CoinsEarned);
        CurrencyManager.Instance.AddCoins(scoreManager.CoinsCollected);
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
