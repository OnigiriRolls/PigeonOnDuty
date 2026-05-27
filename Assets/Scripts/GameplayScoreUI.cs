using TMPro;
using UnityEngine;

public class GameplayScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateUI;
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance == null)
            return;
        ScoreManager.Instance.OnScoreChanged -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI(ScoreManager.Instance.CurrentScore);
    }

    private void UpdateUI(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
