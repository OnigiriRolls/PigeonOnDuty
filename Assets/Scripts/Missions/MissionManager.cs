using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private bool missionRunning;

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
        {
            return;
        }
        if (missionControllers.Contains(controller))
        {
            return;
        }

        missionControllers.Add(controller);
        controller.OnMissionCompleted += CompleteActiveMission;
        if (startMissionAfterLoad)
        {
            StartSelectedMission();
        }
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
        if (SaveManager.Instance.Data.flightTutorialCompleted && !startMissionAfterLoad)
            RequestMissionSelection();
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
        if (missionRunning)
            return;
        if (selectionUI == null)
            return;
        selectionRequested = true;
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
        rewardManager.ResetCurentRewards();
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
            MissionCity.Random => "RandomCityScene",
            _ => throw new ArgumentOutOfRangeException(nameof(city), city, null)
        };
    }

    private void StartSelectedMission()
    {
        IMissionController controller = GetControllerForMission(ActiveMission);
        if (controller == null)
            return;
        missionRunning = true;
        if (EnemyAggroManager.Instance != null)
            EnemyAggroManager.Instance.ResetAggro();
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
        available = available
            .Where(m => unlockManager.IsUnlocked(m.unlockId))
            .Reverse()
            .ToList();
        return available;
    }

    public void CompleteActiveMission()
    {
        if (ActiveMission == null)
            return;
        missionRunning = false;
        IMissionController controller = GetControllerForMission(ActiveMission);
        controller.CompleteMission();
        rewardManager.AddReputation(ActiveMission.reputationReward);
        rewardManager.AddCoins(ActiveMission.coinReward);
        SaveManager.Instance.SaveRunResults(rewardManager.CurrentReputation, rewardManager.CurrentCoins);
        missionRewardUI.ShowReward(rewardManager.CurrentReputation, rewardManager.CurrentCoins);
        ActiveMission = null;
        RequestMissionSelection();
    }

    private void HandleMissionTimerExpired()
    {
        if (ActiveMission == null)
            return;
        missionRunning = false;
        IMissionController controller = GetControllerForMission(ActiveMission);
        controller.FailMission();
        GameManager.Instance.GameOver(DeathReason.TimeUp);
    }

    public void SetMissionRunning(bool running)
    {
        missionRunning = running;
    }

    public void SetCurrentCity(MissionCity currentCity)
    {
        this.currentCity = currentCity;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        if (MissionTimer.Instance != null)
            MissionTimer.Instance.OnTimerExpired -= HandleMissionTimerExpired;
    }
}
