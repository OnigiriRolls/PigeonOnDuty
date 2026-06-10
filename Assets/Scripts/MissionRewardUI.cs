using TMPro;
using UnityEngine;

public class MissionRewardUI : MonoBehaviour
{
    [SerializeField] private GameObject reward;
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private TMP_Text scoreText;
    //[SerializeField] private Animator animator;
    [SerializeField] private AudioClip rewardClip;

    public void ShowReward(string missionName, int score)
    {
        missionText.text = $"{missionName.ToUpperInvariant()} delivered";
        scoreText.text = $"+{score} score";
        reward.SetActive(true);
        if (rewardClip != null)
            AudioManager.Instance.PlaySFX(rewardClip);
        //animator.SetTrigger("Show");
    }

    public void HideReward()
    {
        reward.SetActive(false);
    }
}
