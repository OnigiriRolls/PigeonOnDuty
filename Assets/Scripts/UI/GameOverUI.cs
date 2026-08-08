using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reputationText;
    [SerializeField] private TextMeshProUGUI coinsEarnedText;
    [SerializeField] private TextMeshProUGUI previousCoinsText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private TextMeshProUGUI deathReasonText;
    [SerializeField] private AudioClip gameOverMusic;

    private void OnEnable()
    {
        RewardManager rewardManager = FindAnyObjectByType<RewardManager>();
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        deathReasonText.text = GetDeathReasonText(gameManager.LastDeathReason);
        reputationText.text = $"Reputation: {rewardManager.Reputation}";
        coinsEarnedText.text = $"Coins: {rewardManager.TotalCoins}";
        previousCoinsText.text = $"Previous Coins: {rewardManager.PreviousCoins}";
        totalCoinsText.text = $"Total Coins: {SaveManager.Instance.TotalCoins}";
        AudioManager.Instance.PlayMusic(gameOverMusic);
    }

    public string GetDeathReasonText(DeathReason reason)
    {
        switch (reason)
        {
            case DeathReason.Crow:
                return "The crow got you :(";
            case DeathReason.Bullet:
                return "A bullet brought you down :(";
            case DeathReason.Balloon:
                return "The balloon blew you up :(";
            case DeathReason.NPCTrustLost:
                return "The Human was fed up :(";
            default:
                return "You ran out of time :(";
        }
    }
}
