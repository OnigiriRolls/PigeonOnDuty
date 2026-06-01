using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public event Action<int> OnCoinsChanged;
    public int Coins { get; private set; }
    public int PreviousCoins { get; private set; }

    private const string CoinsKey = "Coins";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCoins();
    }

    public void AddCoins(int amount)
    {
        PreviousCoins = Coins;
        Coins += amount;
        SaveCoins();
        OnCoinsChanged?.Invoke(Coins);
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsKey, Coins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        Coins = PlayerPrefs.GetInt(CoinsKey, 0);
        OnCoinsChanged?.Invoke(Coins);
    }
}
