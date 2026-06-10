using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI coinsEarnedText;
    [SerializeField] private TextMeshProUGUI previousCoinsText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private TextMeshProUGUI deathReasonText;
    [SerializeField] private AudioClip gameOverMusic;

    private void OnEnable()
    {
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        deathReasonText.text = GetDeathReasonText(gameManager.LastDeathReason);
        finalScoreText.text = $"Score: {scoreManager.FinalScore}";
        coinsEarnedText.text = $"Coins: {scoreManager.CoinsEarned}";
        previousCoinsText.text = $"Previous Coins: {scoreManager.PreviousCoins}";
        totalCoinsText.text = $"Total Coins: {SaveManager.Instance.TotalCoins}";
        AudioManager.Instance.PlayMusic(gameOverMusic);
    }

    public string GetDeathReasonText(DeathReason reason)
    {
        switch (reason)
        {
            case DeathReason.Crow:
                return "The crow got you :(";
            case DeathReason.Bullet:
                return "A bullet brought you down :(";
            case DeathReason.Balloon:
                return "The balloon blew you up :(";
            default:
                return "You ran out of time :(";
        }
    }
}
