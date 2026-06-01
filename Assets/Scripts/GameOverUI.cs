using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI coinsEarnedText;
    [SerializeField] private TextMeshProUGUI previousCoinsText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;


    private void OnEnable()
    {
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        finalScoreText.text = $"Score: {scoreManager.FinalScore}";
        coinsEarnedText.text = $"Coins: {scoreManager.CoinsEarned}";
        previousCoinsText.text = $"Previous Coins: {CurrencyManager.Instance.PreviousCoins}";
        totalCoinsText.text = $"Total Coins: {CurrencyManager.Instance.Coins}";
    }
}
