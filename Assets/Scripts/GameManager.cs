using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsGameOver { get; private set; }
    public int FinalScore { get; private set; }
    public int CoinsEarned { get; private set; }

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private AudioClip gameOverMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (IsGameOver)
            return;

        FinalScore = ScoreManager.Instance.CurrentScore;
        Debug.Log("Final Score: " + FinalScore);
        CoinsEarned = FinalScore / 100;
        CurrencyManager.Instance.AddCoins(CoinsEarned);
        AudioManager.Instance.StopAllAudio();
        IsGameOver = true;
        AudioManager.Instance.CrossfadeMusic(gameOverMusic);
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }
}
