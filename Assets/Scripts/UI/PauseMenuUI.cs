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
        SceneLoader.Instance.RetryGameplay();
    }

    public void LoadHome()
    {
        SceneLoader.Instance.LoadScene("StartScene");
    }
}
