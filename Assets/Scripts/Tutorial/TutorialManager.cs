using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    private readonly HashSet<string> completedTutorials = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool HasSeen(string tutorialId)
    {
        return completedTutorials.Contains(tutorialId);
    }

    public void TryShow(string tutorialId, string title, string content)
    {
        if (HasSeen(tutorialId))
            return;
        if (TutorialUI.Instance == null)
        {
            Debug.LogWarning("TutorialUI instance was not found.");
            return;
        }

        TutorialUI.Instance.Show(title, content);
        completedTutorials.Add(tutorialId);
    }

    public void Complete(string tutorialId)
    {
        completedTutorials.Add(tutorialId);
    }

    public void ResetTutorials()
    {
        completedTutorials.Clear();
    }
}
