using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private AudioSource movementAudioSource;
    [SerializeField] private AudioSource hitAudioSource;

    [Header("Takeoff / Landing")]
    [SerializeField] private AudioClip[] takeoffLandingClips;

    [Header("Flying / Gliding")]
    [SerializeField] private AudioClip[] flyingClips;

    [Header("Hit")]
    [SerializeField] private AudioClip[] hitClips;

    [Header("Flying Timing")]
    [SerializeField] private float minFlyingSoundDelay = 2f;
    [SerializeField] private float maxFlyingSoundDelay = 5f;

    private float flyingSoundTimer;
    private bool previousFlyingState;

    private void Start()
    {
        ResetFlyingSoundTimer();
    }

    private void Update()
    {
        HandleTakeoffLandingSounds();
        HandleFlyingSounds();
    }

    private void HandleTakeoffLandingSounds()
    {
        if (!previousFlyingState && player.IsFlying)
        {
            PlayRandomClip(takeoffLandingClips, movementAudioSource);
        }
        if (previousFlyingState && !player.IsFlying)
        {
            PlayRandomClip(takeoffLandingClips, movementAudioSource);
        }

        previousFlyingState = player.IsFlying;
    }

    private void HandleFlyingSounds()
    {
        if (!player.IsFlying)
            return;

        flyingSoundTimer -= Time.deltaTime;

        if (flyingSoundTimer <= 0f)
        {
            PlayRandomClip(flyingClips, movementAudioSource);
            ResetFlyingSoundTimer();
        }
    }

    private void ResetFlyingSoundTimer()
    {
        flyingSoundTimer = Random.Range(minFlyingSoundDelay, maxFlyingSoundDelay);
    }

    private void PlayRandomClip(AudioClip[] clips, AudioSource audioSource)
    {
        if (clips.Length == 0)
            return;
        Debug.Log("PlayRandomClip");
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        movementAudioSource.pitch = Random.Range(0.9f, 1.1f);
        movementAudioSource.PlayOneShot(randomClip);
    }

    public void PlayHitClip()
    {
        Debug.Log("PlayHitClip");
        PlayRandomClip(hitClips, hitAudioSource);
    }
}
