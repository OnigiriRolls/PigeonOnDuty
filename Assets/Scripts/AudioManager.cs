using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip CurrentMusicClip { get; private set; }

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;

    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambianceSource1;
    [SerializeField] private AudioSource ambianceSource2;
    [SerializeField] private AudioSource uiLoopSource;

    private AudioSource activeAmbianceSource;
    private AudioSource inactiveAmbianceSource;

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

        activeAmbianceSource = ambianceSource1;
        inactiveAmbianceSource = ambianceSource2;
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

    public void PlayUILoop(AudioClip clip)
    {
        if (clip == null)
            return;
        if (uiLoopSource.clip == clip && uiLoopSource.isPlaying)
            return;

        uiLoopSource.Stop();
        uiLoopSource.clip = clip;
        uiLoopSource.volume = sfxVolume * masterVolume;
        uiLoopSource.Play();
    }

    public void StopUILoop()
    {
        uiLoopSource.Stop();
    }

    public void CrossfadeMusic(AudioClip newClip, float fadeDuration = 2f)
    {
        if (CurrentMusicClip == newClip)
            return;
        StartCoroutine(CrossfadeCoroutine(newClip, fadeDuration));
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip, float duration)
    {
        inactiveAmbianceSource.clip = newClip;
        CurrentMusicClip = newClip;
        inactiveAmbianceSource.volume = 0f;
        inactiveAmbianceSource.Play();

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            activeAmbianceSource.volume = Mathf.Lerp(musicVolume * masterVolume, 0f, t);
            inactiveAmbianceSource.volume = Mathf.Lerp(0f, musicVolume * masterVolume, t);
            yield return null;
        }

        activeAmbianceSource.Stop();
        (inactiveAmbianceSource, activeAmbianceSource) = (activeAmbianceSource, inactiveAmbianceSource);
    }

    public void StopAllAudio()
    {
        sfxSource.Stop();
        musicSource.Stop();
        ambianceSource1.Stop();
        ambianceSource2.Stop();
        uiLoopSource.Stop();
    }
}
