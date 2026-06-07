using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI coinsEarnedText;
    [SerializeField] private TextMeshProUGUI previousCoinsText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private AudioClip gameOverMusic;

    private void OnEnable()
    {
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        finalScoreText.text = $"Score: {scoreManager.FinalScore}";
        coinsEarnedText.text = $"Coins: {scoreManager.CoinsEarned}";
        previousCoinsText.text = $"Previous Coins: {scoreManager.PreviousCoins}";
        totalCoinsText.text = $"Total Coins: {SaveManager.Instance.TotalCoins}";
        AudioManager.Instance.PlayMusic(gameOverMusic);
    }
}
