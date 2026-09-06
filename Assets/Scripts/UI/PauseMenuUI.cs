using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Start()
    {
        GameManager.Instance.OnPaused += Show;
        GameManager.Instance.OnResumed += Hide;
    }

    private void Show()
    {
        panel.SetActive(true);
    }

    private void Hide()
    {
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPaused -= Show;
        GameManager.Instance.OnResumed -= Hide;
    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }

    public void RetryGame()
    {
        GameManager.Instance.ResetRun();
        MissionManager.Instance.SetMissionRunning(false);
        SceneLoader.Instance.RetryGameplay();
    }

    public void LoadHome()
    {
        MissionManager.Instance.SetMissionRunning(false);
        SceneLoader.Instance.LoadHomeScene();
    }
}
