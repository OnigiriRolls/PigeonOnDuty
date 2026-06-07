using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite normalHeartSprite;
    [SerializeField] private Sprite fragileHeartSprite;
    [SerializeField] private DeliveryMissionManager deliveryMissionManager;

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

    public void Refresh()
    {
        UpdateUI(playerHealth.CurrentHealth);
    }

    private void UpdateUI(int currentHealth)
    {
        Debug.Log($"one hit fail = {deliveryMissionManager.ActiveMission?.oneHitFail}");
        if (deliveryMissionManager.ActiveMission?.oneHitFail == true)
        {
            hearts[0].sprite = fragileHeartSprite;
            hearts[0].transform.localScale = Vector3.one;
            hearts[0].gameObject.SetActive(true);
            for (int i = 1; i < hearts.Length; i++)
            {
                hearts[i].gameObject.SetActive(false);
            }
            return;
        }

        hearts[0].sprite = normalHeartSprite;
        hearts[0].transform.localScale = Vector3.one * 0.7f;
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < currentHealth);
        }
    }
}
