using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnGameOver;
    public bool IsGameOver { get; private set; }

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
}
