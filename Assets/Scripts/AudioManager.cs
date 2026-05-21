using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volumes")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    public void PlayRandomSFX(AudioClip[] clips)
    {
        if (clips.Length == 0)
            return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlaySFX(clip);
    }

    public void Play3DSFX(AudioClip clip, Vector3 position, float spatialBlend = 1f)
    {
        if (clip == null)
            return;
        GameObject tempAudio = new("TempAudio");
        tempAudio.transform.position = position;
        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = spatialBlend;
        source.volume = sfxVolume * masterVolume;
        source.Play();
        Destroy(tempAudio, clip.length);
    }
}
