using UnityEngine;

public class NewsMissionUI : MonoBehaviour
{
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private GameObject inventoryUI;

    private void OnEnable()
    {
        missionManager.OnMissionSelected += HandleMissionSelected;
        UpdateUI(missionManager.ActiveMission);
    }

    private void OnDisable()
    {
        missionManager.OnMissionSelected -= HandleMissionSelected;
    }

    private void HandleMissionSelected(MissionData mission)
    {
        UpdateUI(mission);
    }

    private void UpdateUI(MissionData mission)
    {
        bool isNewsMission = mission is NewsMission;
        inventoryUI.SetActive(isNewsMission);
    }
}
