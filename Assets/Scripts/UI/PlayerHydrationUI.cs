using UnityEngine;
using UnityEngine.UI;

public class PlayerHydrationUI : MonoBehaviour
{
    [SerializeField] private PlayerHydration hydration;
    [SerializeField] private Image hydrationBar;
    [SerializeField] private GameObject hydrationPanel;

    private void OnEnable()
    {
        hydration.OnHydrationChanged += UpdateHydration;
        UpdateHydration(hydration.CurrentHydration, hydration.MaxHydration);
        hydrationPanel.SetActive(true);
    }

    private void OnDisable()
    {
        if (hydrationPanel != null)
            hydrationPanel.SetActive(false);
        hydration.OnHydrationChanged -= UpdateHydration;
    }

    private void UpdateHydration(float current, float max)
    {
        hydrationBar.fillAmount = current / max;
    }
}
