using TMPro;
using UnityEngine;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text missionNameText;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject completedIcon;

    public void Setup(MissionData mission, UnlockManager unlockManager)
    {
        if (mission == null)
            return;
        missionNameText.text = mission.missionName;
        bool unlocked = unlockManager != null && unlockManager.IsUnlocked(mission.unlockId);
        lockIcon.SetActive(!unlocked);
        completedIcon.SetActive(unlocked);
    }
}
