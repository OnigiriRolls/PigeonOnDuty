using TMPro;
using UnityEngine;

public class FlightControlsUI : MonoBehaviour
{
    [SerializeField] private GameObject flightPanel;
    [SerializeField] private GameObject newsPanel;

    private void Awake()
    {
        HideFlightPanel();
        HideNewsPanel();
    }

    public void ShowFlightPanel()
    {
        flightPanel.SetActive(true);
    }

    public void HideFlightPanel()
    {
        flightPanel.SetActive(false);
    }

    public void ShowNewsPanel()
    {
        newsPanel.SetActive(true);
    }

    public void HideNewsPanel()
    {
        newsPanel.SetActive(false);
    }
}
