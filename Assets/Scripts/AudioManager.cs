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
    [SerializeField] private AudioSource environmentalSource;

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

    public void PlayRandomSFX(AudioClip clip, AudioSource source)
    {
        if (clip == null)
            return;
        if (source.clip == clip && source.isPlaying)
            return;

        source.Stop();
        source.clip = clip;
        source.volume = sfxVolume * masterVolume;
        source.Play();
    }

    public void PlayRandomSFX(AudioClip[] clips, AudioSource source)
    {
        if (clips.Length == 0)
            return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlayRandomSFX(clip, source);
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

    public void PlayEnvironmentalLoop(AudioClip clip)
    {
        if (clip == null)
            return;
        if (environmentalSource.clip == clip && environmentalSource.isPlaying)
            return;

        environmentalSource.Stop();
        environmentalSource.clip = clip;
        environmentalSource.loop = true;
        environmentalSource.volume = sfxVolume * masterVolume;
        environmentalSource.Play();
    }

    public void StopEnvironmentalLoop()
    {
        environmentalSource.Stop();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null)
            return;
        if (clip == CurrentMusicClip && activeAmbianceSource.isPlaying)
            return;
        activeAmbianceSource.Stop();
        inactiveAmbianceSource.Stop();
        activeAmbianceSource.clip = clip;
        activeAmbianceSource.loop = loop;
        activeAmbianceSource.volume = musicVolume * masterVolume;
        activeAmbianceSource.Play();
        CurrentMusicClip = clip;
    }

    public void StopAllAudio()
    {
        if (sfxSource != null)
            sfxSource.Stop();
        if (musicSource != null)
            musicSource.Stop();
        if (ambianceSource1 != null)
            ambianceSource1.Stop();
        if (ambianceSource2 != null)
            ambianceSource2.Stop();
        if (uiLoopSource != null)
            uiLoopSource.Stop();
        if (environmentalSource != null)
            environmentalSource.Stop();
        if (CurrentMusicClip != null)
            CurrentMusicClip = null;
    }
}
