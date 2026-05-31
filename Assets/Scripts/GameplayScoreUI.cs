using TMPro;
using UnityEngine;

public class GameplayScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += UpdateUI;
            UpdateUI(scoreManager.CurrentScore);
        }
    }

    private void UpdateUI(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    private void OnDestroy()
    {
        if (scoreManager == null)
            return;
        scoreManager.OnScoreChanged -= UpdateUI;
    }
}
