using System;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public event Action<int> OnRunCoinsChanged;
    public event Action<int> OnReputationChanged;
    public int Reputation { get; private set; }
    public int PreviousCoins { get; set; }
    public int TotalCoins { get; private set; }

    public void AddReputation(int amount)
    {
        Reputation += amount;
        OnReputationChanged?.Invoke(Reputation);
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        OnRunCoinsChanged?.Invoke(TotalCoins);
    }
}
