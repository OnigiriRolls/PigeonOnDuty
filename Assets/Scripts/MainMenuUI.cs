using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;

    void Start()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlayMusic(menuMusic);
    }

    public void StartGame()
    {
        SceneLoader.Instance.LoadScene("DefaultCityScene");
    }

    public void ExitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
