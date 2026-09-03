using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private AudioSource movementAudioSource;
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioClip[] flyingClips;
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private float minFlyingSoundDelay = 2f;
    [SerializeField] private float maxFlyingSoundDelay = 5f;

    private float flyingSoundTimer;

    private void Start()
    {
        ResetFlyingSoundTimer();
    }

    private void Update()
    {
        HandleFlyingSounds();
    }

    private void HandleFlyingSounds()
    {
        flyingSoundTimer -= Time.deltaTime;
        if (flyingSoundTimer <= 0f)
        {
            AudioManager.Instance.PlayRandomSFX(flyingClips, movementAudioSource);
            ResetFlyingSoundTimer();
        }
    }

    private void ResetFlyingSoundTimer()
    {
        flyingSoundTimer = Random.Range(minFlyingSoundDelay, maxFlyingSoundDelay);
    }

    public void PlayHitClip()
    {
        AudioManager.Instance.PlayRandomSFX(hitClips, hitAudioSource);
    }
}
