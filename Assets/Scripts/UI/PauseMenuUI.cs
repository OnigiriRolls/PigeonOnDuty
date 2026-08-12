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
}
