using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MissionManager : MonoBehaviour
{
    public MissionData ActiveMission { get; private set; }
    public bool HasActiveMission => ActiveMission != null;
    public GPSMissionController GPSMissionController => gpsMissionController;
    public NewsMissionController NewsMissionController => newsMissionController;
    public NormalMissionController NormalMissionController => normalMissionController;
    public MissionTimer MissionTimer => missionTimer;
    public event Action<List<MissionData>> OnMissionSelectionRequested;
    public event Action<MissionData> OnMissionSelected;

    [SerializeField] private float selectionInvulnerabilityDuration = 2f;
    [SerializeField] private MissionRewardUI missionRewardUI;
    [SerializeField] private UnlockMessageUI unlockMessageUI;
    [SerializeField] private MissionDatabase missionDatabase;
    [SerializeField] private GPSMissionController gpsMissionController;
    [SerializeField] private NewsMissionController newsMissionController;
    [SerializeField] private NormalMissionController normalMissionController;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private bool isNewsMissionActive;
    [SerializeField] private bool isGPSMissionActive;
    [SerializeField] private bool areAllMissionsActive;
    [SerializeField] private MissionTimer missionTimer;

    private RewardManager rewardManager;
    private PlayerHealth playerHealth;
    private PlayerHealthUI playerHealthUI;
    private UnlockManager unlockManager;

    private void Start()
    {
        rewardManager = FindAnyObjectByType<RewardManager>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerHealthUI = FindAnyObjectByType<PlayerHealthUI>();
        unlockManager = FindAnyObjectByType<UnlockManager>();
        newsMissionController.OnMissionCompleted += CompleteActiveMission;
        missionTimer.OnTimerExpired += HandleMissionTimerExpired;
        RequestMissionSelection();
    }

    public void RequestMissionSelection()
    {
        List<MissionData> missions = GenerateMissionChoices();
        OnMissionSelectionRequested?.Invoke(missions);
        Time.timeScale = 0f;
    }

    public void SelectMission(MissionData mission)
    {
        ActiveMission = mission;
        mission.StartMission(this);
        OnMissionSelected?.Invoke(mission);
        if (playerHealth != null)
            playerHealth.StartInvulnerability(selectionInvulnerabilityDuration);
        if (playerHealthUI != null)
            playerHealthUI.Refresh();
        missionRewardUI.HideReward();
        unlockMessageUI.Hide();
        Time.timeScale = 1f;
    }

    private List<MissionData> GenerateMissionChoices()
    {
        List<MissionData> result = new();
        List<MissionData> available = new(missionDatabase.Missions);
        available = available.Where(m => unlockManager.IsUnlocked(m.unlockId)).ToList();

#if UNITY_EDITOR
        available = available
            .Where(m =>
                (m is NewsMission && isNewsMissionActive) ||
                (m is GPSMission && isGPSMissionActive) ||
                (m is DeliveryMission && areAllMissionsActive))
            .ToList();
#endif
        int count = Mathf.Min(3, available.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }
        return result;
    }

    public void CompleteActiveMission()
    {
        if (ActiveMission == null)
            return;
        ActiveMission.CompleteMission(this);
        rewardManager.AddReputation(ActiveMission.reputationReward);
        rewardManager.AddCoins(ActiveMission.coinReward);
        SaveManager.Instance.SaveRunResults(rewardManager.Reputation, rewardManager.TotalCoins);
        missionRewardUI.ShowReward(ActiveMission.reputationReward, ActiveMission.coinReward);
        ActiveMission = null;
        RequestMissionSelection();
    }

    private void HandleMissionTimerExpired()
    {
        if (ActiveMission == null)
            return;
        ActiveMission.FailMission(this);
        gameManager.GameOver(DeathReason.TimeUp);
    }

    private void OnDestroy()
    {
        if (newsMissionController != null)
            newsMissionController.OnMissionCompleted -= CompleteActiveMission;
        missionTimer.OnTimerExpired -= HandleMissionTimerExpired;
    }
}
