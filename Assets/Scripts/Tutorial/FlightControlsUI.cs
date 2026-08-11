using TMPro;
using UnityEngine;

public class FlightControlsUI : MonoBehaviour
{
    [SerializeField] private GameObject flightPanel;
    [SerializeField] private TextMeshProUGUI[] flightTexts;
    [SerializeField] private string[] flightControlNames;
    [SerializeField] private GameObject newsPanel;
    [SerializeField] private TextMeshProUGUI[] newsTexts;
    [SerializeField] private string[] newsControlNames;

    private void Awake()
    {
        HideFlightPanel();
        HideNewsPanel();
    }

    public void ShowFlightPanel()
    {
        string controlName = InputDisplayHelper.Instance.GetCompositePartDisplayName(flightControlNames[0], "positive");
        flightTexts[0].text = controlName;
         controlName = InputDisplayHelper.Instance.GetCompositePartDisplayName(flightControlNames[1], "negative");
        flightTexts[1].text = controlName;
         controlName = InputDisplayHelper.Instance.GetCompositePartDisplayName(flightControlNames[2], "negative");
        flightTexts[2].text = controlName;
         controlName = InputDisplayHelper.Instance.GetCompositePartDisplayName(flightControlNames[3], "positive");
        flightTexts[3].text = controlName;

        for (int i = 4; i < flightTexts.Length; i++)
        {
            controlName = InputDisplayHelper.Instance.GetDisplayName(flightControlNames[i]);
            flightTexts[i].text = controlName;
            if (i == 7)
            {
                controlName = InputDisplayHelper.Instance.GetDisplayName(flightControlNames[i + 1]);
                flightTexts[i].text += $", {controlName}";
                controlName = InputDisplayHelper.Instance.GetDisplayName(flightControlNames[i + 2]);
                flightTexts[i].text += $", {controlName}";
            }
        }
        flightPanel.SetActive(true);
    }

    public void HideFlightPanel()
    {
        flightPanel.SetActive(false);
    }

    public void ShowNewsPanel()
    {
        for (int i = 0; i < newsTexts.Length; i++)
        {
            string controlName = InputDisplayHelper.Instance.GetDisplayName(newsControlNames[i]);
            newsTexts[i].text = controlName;
        }
        newsPanel.SetActive(true);
    }

    public void HideNewsPanel()
    {
        newsPanel.SetActive(false);
    }
}
