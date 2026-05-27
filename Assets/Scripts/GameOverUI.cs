using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI coinsEarnedText;

    private void OnEnable()
    {
        finalScoreText.text = $"Score: {GameManager.Instance.FinalScore}";
        coinsEarnedText.text = $"Coins: {GameManager.Instance.CoinsEarned}";
    }
}
