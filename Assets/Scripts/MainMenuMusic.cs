using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;

    void Start()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlayMusic(menuMusic);
    }
}
