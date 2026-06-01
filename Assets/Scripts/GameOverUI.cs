using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI coinsEarnedText;

    private void OnEnable()
    {
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        finalScoreText.text = $"Score: {scoreManager.FinalScore}";
        coinsEarnedText.text = $"Coins: {scoreManager.CoinsEarned}";
    }
}
