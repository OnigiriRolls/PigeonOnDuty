using System;
using UnityEngine;

public class PlayerHydration : MonoBehaviour
{
    public float CurrentHydration { get; private set; }
    public float MaxHydration => maxHydration;
    public bool IsActive { get; private set; }
    public event Action<float, float> OnHydrationChanged;

    [SerializeField] private float maxHydration = 100f;
    [SerializeField] private float drainPerSecond = 2f;
    [SerializeField] private float lowHydrationThreshold = 25f;
    [SerializeField] private AudioClip lowHydrationWarningClip;
    [SerializeField] private AudioSource audioSource;

    private bool lowHydrationWarningTriggered;
    private bool dehydrated;

    private void Awake()
    {
        CurrentHydration = maxHydration;
    }

    private void Update()
    {
        if (!IsActive || dehydrated)
            return;
        DrainHydration();
        HandleLowHydrationWarning();
    }

    private void DrainHydration()
    {
        CurrentHydration -= drainPerSecond * Time.deltaTime;
        CurrentHydration = Mathf.Max(CurrentHydration, 0f);
        OnHydrationChanged?.Invoke(CurrentHydration, maxHydration);
        if (CurrentHydration <= 0f)
            BecomeDehydrated();
    }

    private void HandleLowHydrationWarning()
    {
        if (CurrentHydration > lowHydrationThreshold)
        {
            if (lowHydrationWarningTriggered)
            {
                lowHydrationWarningTriggered = false;
                audioSource.Stop();
            }
            return;
        }

        if (lowHydrationWarningTriggered)
            return;

        lowHydrationWarningTriggered = true;
        PlayWarningSound();
    }

    private void PlayWarningSound()
    {
        AudioManager.Instance.PlayRandomSFX(lowHydrationWarningClip, audioSource);
    }

    private void BecomeDehydrated()
    {
        if (dehydrated)
            return;
        dehydrated = true;
        IsActive = false;
        audioSource.Stop();
        GameManager.Instance.GameOver(DeathReason.Dehydration);
    }

    public void Activate()
    {
        IsActive = true;
        dehydrated = false;
        CurrentHydration = maxHydration;
        lowHydrationWarningTriggered = false;
        audioSource.Stop();
        OnHydrationChanged?.Invoke(CurrentHydration, maxHydration);
    }

    public void Deactivate()
    {
        IsActive = false;
        audioSource.Stop();
    }

    public void RestoreHydration(float amount)
    {
        if (!IsActive || dehydrated)
            return;
        CurrentHydration = Mathf.Clamp(CurrentHydration + amount, 0f, maxHydration);
        if (CurrentHydration > lowHydrationThreshold)
        {
            lowHydrationWarningTriggered = false;
            audioSource.Stop();
        }
        OnHydrationChanged?.Invoke(CurrentHydration, maxHydration);
    }
}
