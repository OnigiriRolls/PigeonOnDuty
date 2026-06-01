using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsGameOver { get; private set; }

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private AudioClip gameOverMusic;

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
        AudioManager.Instance.StopAllAudio();
        IsGameOver = true;
        AudioManager.Instance.CrossfadeMusic(gameOverMusic);
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }
}
