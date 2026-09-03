using TMPro;
using UnityEngine;

public class GameplayCoinsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;

    private RewardManager rewardManager;

    private void Start()
    {
        rewardManager = FindAnyObjectByType<RewardManager>();
        rewardManager.OnRunCoinsChanged += UpdateUI;
        UpdateUI(GameManager.Instance.TotalCoins);
    }

    private void UpdateUI(int coins)
    {
        coinsText.text = $"{coins}";
    }

    private void OnDestroy()
    {
        if (rewardManager == null)
            return;
        rewardManager.OnRunCoinsChanged -= UpdateUI;
    }
}
