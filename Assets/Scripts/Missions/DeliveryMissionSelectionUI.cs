using System.Collections.Generic;
using UnityEngine;

public class DeliveryMissionSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private DeliveryMissionCardUI missionCardPrefab;
    [SerializeField] private Transform content;

    private readonly List<DeliveryMissionCardUI> spawnedCards = new();

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
        ClearCards();

        foreach (MissionData mission in missions)
        {
            DeliveryMissionCardUI card = Instantiate(missionCardPrefab, content);
            card.Setup(mission, OnMissionSelected);
            spawnedCards.Add(card);
        }
        panel.SetActive(true);
    }

    private void ClearCards()
    {
        foreach (DeliveryMissionCardUI card in spawnedCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        spawnedCards.Clear();
    }

    private void OnMissionSelected(MissionData mission)
    {
        panel.SetActive(false);
        MissionManager.Instance.SelectMission(mission);
    }

    private void OnDestroy()
    {
        ClearCards();
        if (MissionManager.Instance != null)
            MissionManager.Instance.UnregisterSelectionUI(this);
    }
}
