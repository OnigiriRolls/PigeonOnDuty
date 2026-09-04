using TMPro;
using UnityEngine;

public class CareerUI : MonoBehaviour
{
    [SerializeField] private CityDatabase cityDatabase;
    [SerializeField] private Transform cityContainer;
    [SerializeField] private CityUI cityUIPrefab;
    [SerializeField] private TMP_Text reputationText;
    [SerializeField] private UnlockManager unlockManager;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        reputationText.text =$"{SaveManager.Instance.Reputation}";
        foreach (Transform child in cityContainer)
            Destroy(child.gameObject);
        foreach (CityData city in cityDatabase.cities)
        {
            CityUI cityUI = Instantiate(cityUIPrefab, cityContainer);
            cityUI.Setup(city, unlockManager);
        }
    }
}
