using TMPro;
using UnityEngine;

public class GameplayRewardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reputationText;
    private RewardManager rewardManager;

    private void Start()
    {
        rewardManager = FindAnyObjectByType<RewardManager>();
        if (rewardManager != null)
        {
            rewardManager.OnReputationChanged += UpdateUI;
            UpdateUI(GameManager.Instance.TotalReputation);
        }
    }

    private void UpdateUI(int reward)
    {
        reputationText.text = $"{reward}";
    }

    private void OnDestroy()
    {
        if (rewardManager == null)
            return;
        rewardManager.OnReputationChanged -= UpdateUI;
    }
}
