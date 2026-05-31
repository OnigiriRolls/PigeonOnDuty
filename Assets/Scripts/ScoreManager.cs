using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action<int> OnScoreChanged;
    public int CurrentScore { get; private set; }

    [SerializeField] private int checkpointScore = 100;
    [SerializeField] private int lowAltitudeScorePerSecond = 1;
    [SerializeField] private int midAltitudeScorePerSecond = 2;
    [SerializeField] private int highAltitudeScorePerSecond = 3;

    private GameplayUI gameplayUI;
    private float scoreTimer;

    private void Start()
    {
        gameplayUI = FindAnyObjectByType<GameplayUI>();
        ResetScore();
    }

    private void Update()
    {
        Debug.Log("ok1");
        scoreTimer += Time.deltaTime;
        if (scoreTimer < 1f)
            return;
        Debug.Log("ok2");
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
}
