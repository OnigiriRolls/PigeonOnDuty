using System;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public event Action<int> OnRunCoinsChanged;
    public event Action<int> OnReputationChanged;
    public int Reputation { get; private set; }
    public int PreviousCoins { get; set; }
    public int TotalCoins { get; private set; }
    public int CurentCoins { get; private set; }
    public int CurentReputation { get; private set; }

    public void AddReputation(int amount)
    {
        Reputation += amount;
        CurentReputation += amount;
        OnReputationChanged?.Invoke(Reputation);
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        CurentCoins += amount;
        OnRunCoinsChanged?.Invoke(TotalCoins);
    }

    public void ResetCurentRewards()
    {
        CurentCoins = 0;
        CurentReputation = 0;
    }
}
