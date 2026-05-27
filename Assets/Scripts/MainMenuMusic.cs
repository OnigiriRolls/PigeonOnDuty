using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;

    void Start()
    {
        AudioManager.Instance.CrossfadeMusic(menuMusic);
    }
}
