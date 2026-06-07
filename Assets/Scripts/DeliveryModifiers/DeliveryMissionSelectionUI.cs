using System.Collections.Generic;
using UnityEngine;

public class DeliveryMissionSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private DeliveryMissionCardUI[] missionCards;
    [SerializeField] private DeliveryMissionManager manager;

    private void Start()
    {
        panel.SetActive(false);
        manager.OnMissionSelectionRequested += ShowMissionSelection;
    }

    private void ShowMissionSelection(List<DeliveryMission> missions)
    {
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

    private void OnMissionSelected(DeliveryMission mission)
    {
        panel.SetActive(false);
        manager.SelectMission(mission);
    }

    private void OnDestroy()
    {
        if (manager != null)
            manager.OnMissionSelectionRequested -= ShowMissionSelection;
    }
}
