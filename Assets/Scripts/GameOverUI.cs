using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI coinsEarnedText;

    private void OnEnable()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        finalScoreText.text = $"Score: {gameManager.FinalScore}";
        coinsEarnedText.text = $"Coins: {gameManager.CoinsEarned}";
    }
}
