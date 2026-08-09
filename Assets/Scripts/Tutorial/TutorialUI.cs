using System;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public bool IsShowing => tutorialPanel.activeSelf;
    public static TutorialUI Instance { get; private set; }
    public event Action OnTutorialClosed;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Hide();
    }

    public void Show(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;
        tutorialPanel.SetActive(true);
        PauseManager.Instance.Pause(PauseReason.TutorialMessage);
    }

    public void Hide()
    {
        if (!tutorialPanel.activeSelf)
            return;
        tutorialPanel.SetActive(false);
        PauseManager.Instance.Resume(PauseReason.TutorialMessage);
        OnTutorialClosed?.Invoke();
    }
}
