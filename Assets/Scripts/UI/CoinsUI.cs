using TMPro;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text totalCoinsText;

    private void OnEnable()
    {
        if (SaveManager.Instance == null)
            return;
        SaveManager.Instance.OnCoinsChanged += UpdateCoins;
        UpdateCoins(SaveManager.Instance.TotalCoins);
    }

    private void OnDisable()
    {
        if (SaveManager.Instance == null)
            return;
        SaveManager.Instance.OnCoinsChanged -= UpdateCoins;
    }

    private void UpdateCoins(int amount)
    {
        totalCoinsText.text = $"{amount}";
    }
}
