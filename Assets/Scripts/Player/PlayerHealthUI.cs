using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] hearts;

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI(playerHealth.CurrentHealth);
    }

    private void UpdateUI(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < currentHealth);
        }
    }
}
