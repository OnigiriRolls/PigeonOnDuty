using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public bool IsShowing => TutorialUI.Instance != null &&  TutorialUI.Instance.IsShowing;

    private readonly HashSet<string> completedTutorials = new();
    private string currentTutorialId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (TutorialUI.Instance != null)
            TutorialUI.Instance.OnTutorialClosed += CompleteCurrentTutorial;
    }

    private void OnDisable()
    {
        if (TutorialUI.Instance != null)
            TutorialUI.Instance.OnTutorialClosed -= CompleteCurrentTutorial;
    }

    public bool HasSeen(string tutorialId)
    {
        return completedTutorials.Contains(tutorialId);
    }

    public bool TryShow(string tutorialId, string title, string content)
    {
        if (HasSeen(tutorialId))
            return false;
        if (TutorialUI.Instance == null)
        {
            Debug.LogWarning("TutorialUI instance was not found.");
            return false;
        }
        if (TutorialUI.Instance.IsShowing)
            return false;

        currentTutorialId = tutorialId;
        TutorialUI.Instance.Show(title, content);
        return true;
    }

    private void CompleteCurrentTutorial()
    {
        if (string.IsNullOrEmpty(currentTutorialId))
            return;
        completedTutorials.Add(currentTutorialId);
        currentTutorialId = null;
    }

    public void Complete(string tutorialId)
    {
        completedTutorials.Add(tutorialId);
        if (currentTutorialId == tutorialId)
            currentTutorialId = null;
    }

    public void ResetTutorials()
    {
        completedTutorials.Clear();
        currentTutorialId = null;
    }
}
