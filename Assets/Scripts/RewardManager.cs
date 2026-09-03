using System;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public event Action<int> OnRunCoinsChanged;
    public event Action<int> OnReputationChanged;
    public int CurrentCoins { get; private set; }
    public int CurrentReputation { get; private set; }

    public void AddReputation(int amount)
    {
        GameManager.Instance.TotalReputation += amount;
        CurrentReputation += amount;
        OnReputationChanged?.Invoke(GameManager.Instance.TotalReputation);
    }

    public void AddCoins(int amount)
    {
        GameManager.Instance.TotalCoins += amount;
        CurrentCoins += amount;
        OnRunCoinsChanged?.Invoke(GameManager.Instance.TotalCoins);
    }

    public void ResetCurentRewards()
    {
        CurrentCoins = 0;
        CurrentReputation = 0;
    }
}
