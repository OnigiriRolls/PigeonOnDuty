using UnityEngine;

public abstract class StopAudio : MonoBehaviour
{
    private GameManager gameManager;

    protected virtual void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnGameOver += HandleGameOver;
    }

    protected abstract void HandleGameOver();

    protected virtual void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnGameOver -= HandleGameOver;
        }
    }
}
