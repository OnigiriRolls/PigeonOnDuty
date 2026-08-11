using System;
using UnityEngine;

public class PauseInput : MonoBehaviour
{
    [SerializeField] private PlayerInputController input;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        input.OnPause += HandlePause;
    }

    private void OnDisable()
    {
        input.OnPause -= HandlePause;
    }

    private void HandlePause()
    {
        if (gameManager.IsPaused)
            gameManager.ResumeGame();
        else
            gameManager.PauseGame();
    }
}
