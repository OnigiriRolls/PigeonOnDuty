using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reputationText;
    [SerializeField] private TextMeshProUGUI coinsEarnedText;
    [SerializeField] private TextMeshProUGUI deathReasonText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject panel;
    [SerializeField] private AudioClip gameOverMusic;

    private void Awake()
    {
        homeButton.onClick.AddListener(OnHomeClicked);
        retryButton.onClick.AddListener(OnRetryClicked);
    }

    private void OnHomeClicked()
    {
        SceneLoader.Instance.LoadHomeScene();
    }

    private void OnRetryClicked()
    {
        GameManager.Instance.ResetRun();
        SceneLoader.Instance.RetryGameplay();
    }

    public void ShowUI(DeathReason deathReason, int reputation, int totalCoins)
    {
        deathReasonText.text = GetDeathReasonText(deathReason);
        reputationText.text = $"{reputation}";
        coinsEarnedText.text = $"{totalCoins}";
        panel.SetActive(true);
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
            case DeathReason.Dehydration:
                return "The pigeon got dehydrated :(";
            default:
                return "You ran out of time :(";
        }
    }

    private void OnDestroy()
    {
        if (homeButton != null)
            homeButton.onClick.RemoveListener(OnHomeClicked);
        if (retryButton != null)
            retryButton.onClick.RemoveListener(OnRetryClicked);
        AudioManager.Instance.StopAllAudio();
    }
}
