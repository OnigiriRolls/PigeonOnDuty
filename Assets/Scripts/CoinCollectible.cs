using UnityEngine;

public class CoinCollectible : CollectibleBase
{
    [SerializeField] private int coinAmount = 1;

    protected override void Collect(GameObject player)
    {
        RewardManager scoreManager = FindAnyObjectByType<RewardManager>();
        scoreManager.AddCoins(coinAmount);
    }
}