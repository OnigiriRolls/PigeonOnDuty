using TMPro;
using UnityEngine;

public class FlightControlsUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
