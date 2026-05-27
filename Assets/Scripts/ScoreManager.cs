using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public event Action<int> OnScoreChanged;
    public int CurrentScore { get; private set; }

    [SerializeField] private int checkpointScore = 100;
    [SerializeField] private int lowAltitudeScorePerSecond = 1;
    [SerializeField] private int midAltitudeScorePerSecond = 2;
    [SerializeField] private int highAltitudeScorePerSecond = 3;
    [SerializeField] private GameplayUI gameplayUI;

    private float scoreTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver)
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
        }
        return 1;
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
    }
}
