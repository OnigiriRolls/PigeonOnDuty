using System.Collections.Generic;
using UnityEngine;

public class DeliveryMissionSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private DeliveryMissionCardUI[] missionCards;

    private void Start()
    {
        MissionManager.Instance.RegisterSelectionUI(this);
    }

    public void ShowMissionSelection(List<MissionData> missions)
    {
        if (panel == null)
        {
            Debug.Log("panel null");
            return;
        }
        panel.SetActive(true);
        for (int i = 0; i < missionCards.Length; i++)
        {
            if (i >= missions.Count)
            {
                missionCards[i].gameObject.SetActive(false);
                continue;
            }
            missionCards[i].gameObject.SetActive(true);
            missionCards[i].Setup(missions[i], OnMissionSelected);
        }
    }

    private void OnMissionSelected(MissionData mission)
    {
        panel.SetActive(false);
        MissionManager.Instance.SelectMission(mission);
    }

    private void OnDestroy()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.UnregisterSelectionUI(this);
    }
}
