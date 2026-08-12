using TMPro;
using UnityEngine;

public class MissionRewardUI : MonoBehaviour
{
    [SerializeField] private GameObject reward;
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private TMP_Text reputationText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private AudioClip rewardClip;

    public void ShowReward(int reputation, int coins)
    {
        missionText.text = "Mission completed";
        reputationText.text = $"+{reputation}";
        coinsText.text = $"+{coins}";
        reward.SetActive(true);
        if (rewardClip != null)
            AudioManager.Instance.PlaySFX(rewardClip);
    }

    public void HideReward()
    {
        if (reward != null)
            reward.SetActive(false);
    }
}
