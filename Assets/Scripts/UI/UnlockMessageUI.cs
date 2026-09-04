using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UnlockMessageUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text nameText;

    private UnlockManager unlockManager;

    private void Start()
    {
        unlockManager = FindAnyObjectByType<UnlockManager>();
        unlockManager.OnUnlocked += ShowUnlock;
    }

    private void ShowUnlock(List<UnlockableData> unlockables)
    {
        titleText.text = "NEW CONTENT UNLOCKED";
        nameText.text = string.Join(", ", unlockables.Select(u => u.displayName));
        panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (unlockManager != null)
            unlockManager.OnUnlocked -= ShowUnlock;
    }
}
