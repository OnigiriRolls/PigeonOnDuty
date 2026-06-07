using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action<int> OnScoreChanged;
    public event Action<int> OnRunCoinsChanged;
    public int CurrentScore { get; private set; }
    public int FinalScore { get; private set; }
    public int PreviousCoins { get; set; }
    public int CoinsEarned { get; private set; }
    public int CoinsCollected { get; private set; }

    [SerializeField] private int checkpointScore = 100;
    [SerializeField] private int lowAltitudeScorePerSecond = 1;
    [SerializeField] private int midAltitudeScorePerSecond = 2;
    [SerializeField] private int highAltitudeScorePerSecond = 3;
    [SerializeField] private int CoinConversionRate = 100;
    [SerializeField] private DeliveryMissionManager deliveryMissionManager;

    private GameplayUI gameplayUI;
    private float scoreTimer;
    private EndlessRunManager runManager;

    private void Start()
    {
        gameplayUI = FindAnyObjectByType<GameplayUI>();
        runManager = FindAnyObjectByType<EndlessRunManager>();
        ResetScore();
    }

    private void Update()
    {
        if (!runManager.TimerStarted)
            return;
        scoreTimer += Time.deltaTime;
        if (scoreTimer < 1f)
            return;
        scoreTimer = 0f;
        AddScore(GetAltitudeScore());
    }

    public void AddCheckpointScore()
    {
        AddScore(checkpointScore);
    }

    private void AddScore(int amount)
    {
        CurrentScore += amount;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    private int GetAltitudeScore()
    {
        AltitudeLayer layer = gameplayUI.CurrentLayer;
        switch (layer)
        {
            case AltitudeLayer.Low:
                return lowAltitudeScorePerSecond;
            case AltitudeLayer.Mid:
                return midAltitudeScorePerSecond;
            case AltitudeLayer.High:
                return highAltitudeScorePerSecond;
            default:
                break;
        }
        return 1;
    }

    private void ResetScore()
    {
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    public void FinishRun()
    {
        FinalScore = CurrentScore;
        CoinsEarned = Mathf.RoundToInt(FinalScore / CoinConversionRate);
        if (deliveryMissionManager.HasActiveMission)
        {
            CoinsEarned = Mathf.RoundToInt(CoinsEarned * deliveryMissionManager.ActiveMission.coinMultiplier);
        }
    }

    public void AddRunCoins(int amount)
    {
        CoinsCollected += amount;
        OnRunCoinsChanged?.Invoke(CoinsCollected);
    }

    public void AddMissionReward(int amount)
    {
        AddScore(amount);
    }
}
