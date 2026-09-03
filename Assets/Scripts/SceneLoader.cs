using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private Action onSceneLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadHomeScene()
    {
        LoadScene("StartScene");
    }

    public void LoadScene(string sceneName, Action afterLoad)
    {
        onSceneLoaded = afterLoad;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        Action callback = onSceneLoaded;
        onSceneLoaded = null;
        callback?.Invoke();
    }

    public void RetryGameplay()
    {
        PauseManager.Instance.ClearPauses();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
