using TMPro;
using UnityEngine;

public class GameplayCoinsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager.OnRunCoinsChanged += UpdateUI;
        UpdateUI(scoreManager.CoinsCollected);
    }

    private void UpdateUI(int coins)
    {
        coinsText.text = $"Coins: {coins}";
    }

    private void OnDestroy()
    {
        if (scoreManager == null)
            return;
        scoreManager.OnRunCoinsChanged -= UpdateUI;
    }
}
