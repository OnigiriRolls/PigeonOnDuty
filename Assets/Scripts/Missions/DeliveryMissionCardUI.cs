using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryMissionCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button selectButton;

    private MissionData mission;
    private Action<MissionData> onSelected;

    public void Setup(MissionData mission, Action<MissionData> onSelected)
    {
        this.mission = mission;
        this.onSelected = onSelected;
        titleText.text = mission.missionName;
        descriptionText.text = $"{mission.description}{Environment.NewLine}{Environment.NewLine}" +
            $"Note:{Environment.NewLine}{Environment.NewLine}" +
            $"{mission.GetRandomFlavorText()}";
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(SelectMission);
    }

    private void SelectMission()
    {
        onSelected?.Invoke(mission);
    }
}
