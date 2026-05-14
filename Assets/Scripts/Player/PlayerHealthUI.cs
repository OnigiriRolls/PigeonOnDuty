using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI healthText;

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI(int currentHealth)
    {
        healthText.text = $"Lives: {currentHealth}";
    }
}
