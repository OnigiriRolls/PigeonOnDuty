using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    public MissionData ActiveMission { get; private set; }
    public bool StartMissionAfterLoad => startMissionAfterLoad;
    public event Action<MissionData> OnMissionSelected;

    [SerializeField] private float selectionInvulnerabilityDuration = 2f;
    [SerializeField] private MissionDatabase missionDatabase;
    [SerializeField] private bool isNewsMissionActive;
    [SerializeField] private bool isGPSMissionActive;
    [SerializeField] private bool isArabMissionActive;
    [SerializeField] private bool areAllMissionsActive;

    private readonly List<IMissionController> missionControllers = new();
    private RewardManager rewardManager;
    private PlayerHealth playerHealth;
    private PlayerHealthUI playerHealthUI;
    private UnlockManager unlockManager;
    private UnlockMessageUI unlockMessageUI;
    private MissionRewardUI missionRewardUI;
    private DeliveryMissionSelectionUI selectionUI;
    private MissionCity currentCity;
    private bool selectionRequested;
    private bool startMissionAfterLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        RefreshSceneReferences();
        MissionTimer.Instance.OnTimerExpired += HandleMissionTimerExpired;
        currentCity = MissionCity.DefaultCity;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSceneReferences();
    }

    private void RefreshSceneReferences()
    {
        rewardManager = FindAnyObjectByType<RewardManager>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerHealthUI = FindAnyObjectByType<PlayerHealthUI>();
        unlockManager = FindAnyObjectByType<UnlockManager>();
        unlockMessageUI = FindAnyObjectByType<UnlockMessageUI>();
        missionRewardUI = FindAnyObjectByType<MissionRewardUI>();
    }

    public void RegisterController(IMissionController controller)
    {
        if (controller == null)
            return;
        if (missionControllers.Contains(controller))
            return;
        missionControllers.Add(controller);
        controller.OnMissionCompleted += CompleteActiveMission;
        if (startMissionAfterLoad)
            StartSelectedMission();
    }

    public void UnregisterController(IMissionController controller)
    {
        if (controller == null)
            return;
        controller.OnMissionCompleted -= CompleteActiveMission;
        missionControllers.Remove(controller);
    }

    public void RegisterSelectionUI(DeliveryMissionSelectionUI ui)
    {
        selectionUI = ui;
        ShowSelection();
    }

    public void UnregisterSelectionUI(DeliveryMissionSelectionUI ui)
    {
        if (selectionUI == ui)
            selectionUI = null;
    }

    private IMissionController GetControllerForMission(MissionData mission)
    {
        return missionControllers.FirstOrDefault(controller => controller.CanHandle(mission));
    }

    public void RequestMissionSelection()
    {
        selectionRequested = true;
        if (selectionUI == null)
            return;
        ShowSelection();
    }

    private void ShowSelection()
    {
        if (!selectionRequested)
            return;
        List<MissionData> missions = GenerateMissionChoices();
        selectionUI.ShowMissionSelection(missions);
        PauseManager.Instance.Pause(PauseReason.MissionSelection);
        selectionRequested = false;
    }

    public void SelectMission(MissionData mission)
    {
        ActiveMission = mission;
        if (mission.city != currentCity)
        {
            currentCity = mission.city;
            LoadCityForMission(mission);
            return;
        }
        StartSelectedMission();
    }

    private void LoadCityForMission(MissionData mission)
    {
        string sceneName = GetSceneName(mission.city);
        PauseManager.Instance.Resume(PauseReason.MissionSelection);
        startMissionAfterLoad = true;
        SceneLoader.Instance.LoadScene(sceneName, StartSelectedMission);
    }

    private string GetSceneName(MissionCity city)
    {
        return city switch
        {
            MissionCity.DefaultCity => "DefaultCityScene",
            MissionCity.ArabCity => "ArabCityScene",
            _ => throw new ArgumentOutOfRangeException(nameof(city), city, null)
        };
    }

    private void StartSelectedMission()
    {
        IMissionController controller = GetControllerForMission(ActiveMission);
        if (controller == null)
            return;
        controller.StartMission(ActiveMission);
        OnMissionSelected?.Invoke(ActiveMission);
        playerHealth.StartInvulnerability(selectionInvulnerabilityDuration);
        if (playerHealthUI != null)
            playerHealthUI.Refresh();
        missionRewardUI.HideReward();
        unlockMessageUI.Hide();
        startMissionAfterLoad = false;
        PauseManager.Instance.Resume(PauseReason.MissionSelection);
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
                (m is DeliveryMission && areAllMissionsActive) ||
                (m is ArabianNewsMission && isArabMissionActive))
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
        IMissionController controller = GetControllerForMission(ActiveMission);
        controller.CompleteMission();
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
        IMissionController controller = GetControllerForMission(ActiveMission);
        controller.FailMission();
        GameManager.Instance.GameOver(DeathReason.TimeUp);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        if (MissionTimer.Instance != null)
            MissionTimer.Instance.OnTimerExpired -= HandleMissionTimerExpired;
    }
}
