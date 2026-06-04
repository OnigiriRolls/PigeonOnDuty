using UnityEngine;

public class PauseInput : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;
        if (gameManager.IsPaused)
            gameManager.ResumeGame();
        else
            gameManager.PauseGame();
    }
}
