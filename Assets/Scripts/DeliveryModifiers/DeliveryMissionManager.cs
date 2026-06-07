using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryMissionManager : MonoBehaviour
{
    public DeliveryMission ActiveMission { get; private set; }
    public bool HasActiveMission => ActiveMission != null;
    public event Action<List<DeliveryMission>> OnMissionSelectionRequested;

    [SerializeField] private float selectionInvulnerabilityDuration = 2f;

    private ScoreManager scoreManager;
    private PlayerHealth playerHealth;
    private PlayerHealthUI playerHealthUI;

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerHealthUI = FindAnyObjectByType<PlayerHealthUI>();
    }

    public void RequestMissionSelection()
    {
        List<DeliveryMission> missions = GenerateMissionChoices();
        Time.timeScale = 0f;
        OnMissionSelectionRequested?.Invoke(missions);
    }

    public void SelectMission(DeliveryMission mission)
    {
        ActiveMission = mission;
        Debug.Log($"Selected Mission: {mission.missionName}");
        if (playerHealth != null)
            playerHealth.StartInvulnerability(selectionInvulnerabilityDuration);
        if (playerHealthUI != null)
            playerHealthUI.Refresh();
        Time.timeScale = 1f;
    }

    private List<DeliveryMission> GenerateMissionChoices()
    {
        List<DeliveryMission> result = new();
        List<DeliveryMission> available = new(DeliveryMissionDatabase.Instance.Missions);
        int count = Mathf.Min(3, available.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }
        return result;
    }

    public void CompleteMission()
    {
        if (ActiveMission == null)
            return;
        Debug.Log($"+{ActiveMission.checkpointReward} mission reward");
        scoreManager.AddMissionReward(ActiveMission.checkpointReward);
        ActiveMission = null;
    }
}
