using System;
using UnityEngine;

public class PauseInput : MonoBehaviour
{
    [SerializeField] private PlayerInputController input;

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
        if (GameManager.Instance.IsPaused)
            GameManager.Instance.ResumeGame();
        else
            GameManager.Instance.PauseGame();
    }
}
