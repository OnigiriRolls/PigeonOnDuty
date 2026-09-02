using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cityNameText;
    [SerializeField] private TMP_Text requirementText;
    [SerializeField] private TMP_Text comingSoonText;
    [SerializeField] private Image cityImage;
    [SerializeField] private Transform missionContainer;
    [SerializeField] private Transform parentContainer;
    [SerializeField] private MissionUI missionPrefab;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject starIcon;

    public void Setup(CityData city, UnlockManager unlockManager)
    {
        cityNameText.text = city.displayName;
        int reputation = SaveManager.Instance.Reputation;
        cityImage.sprite = city.icon;
        bool unlocked = reputation >= city.unlockReputationRequired;
        lockedOverlay.SetActive(!unlocked);
        parentContainer.gameObject.SetActive(unlocked);
        requirementText.text = $"{city.unlockReputationRequired}";
        starIcon.SetActive(unlocked);
        requirementText.gameObject.SetActive(unlocked);
        comingSoonText.gameObject.SetActive(!unlocked);
        if (!unlocked)
            return;
        foreach (MissionData mission in city.missions)
        {
            MissionUI missionUI = Instantiate(missionPrefab, missionContainer);
            missionUI.Setup(mission, unlockManager);
        }
    }
}
