using UnityEngine;

public class CoinCollectible : CollectibleBase
{
    [SerializeField] private int coinAmount = 1;

    protected override void Collect(GameObject player)
    {
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager.AddRunCoins(coinAmount);
    }
}