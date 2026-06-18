using System;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public event Action<List<UnlockableData>> OnUnlocked;
    public IReadOnlyList<MissionData> UnlockedMissions => unlockedMissions;

    [SerializeField] private UnlockDatabase database;
    [SerializeField] private MissionDatabase missionDatabase;

    private readonly List<UnlockableData> unlocked = new();
    private readonly List<UnlockableData> locked = new();
    private readonly List<MissionData> unlockedMissions = new();

    private void Start()
    {
        unlocked.Clear();
        locked.Clear();
        foreach (UnlockableData unlockable in database.Unlockables)
        {
            if (IsUnlocked(unlockable.id))
            {
                unlocked.Add(unlockable);
                MissionData mission = missionDatabase.GetByUnlockId(unlockable.id);
                if (mission != null)
                    unlockedMissions.Add(mission);
            }
            else
                locked.Add(unlockable);
        }
        CheckUnlocks(SaveManager.Instance.Reputation);
        SaveManager.Instance.OnReputationChanged += OnReputationChanged;
    }

    public void OnReputationChanged(int reputation)
    {
        CheckUnlocks(reputation);
    }

    public void CheckUnlocks(int reputation)
    {
        List<UnlockableData> newlyUnlocked = new();
        for (int i = locked.Count - 1; i >= 0; i--)
        {
            UnlockableData unlockable = locked[i];
            if (reputation < unlockable.requiredReputation)
                continue;
            Unlock(unlockable);
            locked.RemoveAt(i);
            unlocked.Add(unlockable);
            if (unlockable.showUnlockNotification)
                newlyUnlocked.Add(unlockable);
        }

        if (newlyUnlocked.Count > 0)
        {
            OnUnlocked?.Invoke(newlyUnlocked);
        }
    }


    private void Unlock(UnlockableData unlockable)
    {
        SaveManager.Instance.Data.unlockedContentIds.Add(unlockable.id);
        if (unlockable.type == UnlockableType.Mission)
        {
            MissionData mission = missionDatabase.GetByUnlockId(unlockable.id);
            if (mission != null)
                unlockedMissions.Add(mission);
        }

        SaveManager.Instance.Save();
    }

    public bool IsUnlocked(string id)
    {
        return SaveManager.Instance.Data.unlockedContentIds.Contains(id);
    }

    private void OnDestroy()
    {
        SaveManager.Instance.OnReputationChanged -= OnReputationChanged;
    }
}
