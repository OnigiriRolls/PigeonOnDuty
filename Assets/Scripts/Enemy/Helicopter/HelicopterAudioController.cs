using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HelicopterAudioController : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip flyingClip;
    [SerializeField] private AudioClip shootingClip;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFlying()
    {
        PlayClip(flyingClip);
    }

    public void PlayShooting()
    {
        PlayClip(shootingClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource.clip == clip)
            return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
