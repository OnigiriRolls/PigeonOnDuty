using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnPaused += Show;
        gameManager.OnResumed += Hide;
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
        gameManager.OnPaused -= Show;
        gameManager.OnResumed -= Hide;
    }
}
