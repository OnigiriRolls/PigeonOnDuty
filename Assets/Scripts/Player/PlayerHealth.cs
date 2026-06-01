using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerAudioController))]
public class PlayerHealth : MonoBehaviour
{
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public int CurrentHealth { get; private set; }
    public bool HasShield { get; private set; }
    public GameObject HitParticlesPrefab => hitParticlesPrefab;

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject hitParticlesPrefab;
    [SerializeField] private GameObject shield;
    [SerializeField] private ParticleSystem shieldBreakEffect;
    [SerializeField] private AudioClip shieldBreakClip;
    [SerializeField] private float shieldDuration = 10f;
    [SerializeField] private float pulseStartTime = 3f;

    private Coroutine shieldCoroutine;
    private PlayerAudioController audioController;
    private GameManager gameManager;
    private Animator shieldAnimator;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Start()
    {
        audioController = GetComponent<PlayerAudioController>();
        gameManager = FindAnyObjectByType<GameManager>();
        shieldAnimator = shield.GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (HasShield)
        {
            ConsumeShield(true);
            return;
        }

        audioController.PlayHitClip();
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Die()
    {
        //Debug.Log("GAME OVER");
        OnDeath?.Invoke();
        gameManager.GameOver();
    }

    public void ActivateShield()
    {
        HasShield = true;
        shield.SetActive(true);
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldAnimator.SetBool("Pulse", false);
        }
        shieldCoroutine = StartCoroutine(ShieldDurationRoutine());
    }

    private IEnumerator ShieldDurationRoutine()
    {
        float remainingTime = shieldDuration;
        bool pulseStarted = false;
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            if (!pulseStarted && remainingTime <= pulseStartTime)
            {
                pulseStarted = true;
                shieldAnimator.SetBool("Pulse", true);
            }
            yield return null;
        }
        ConsumeShield(false);
    }

    public void ConsumeShield(bool blockedDamage)
    {
        HasShield = false;
        shieldAnimator.SetBool("Pulse", false);
        shield.SetActive(false);
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
        }
        if (shieldBreakEffect != null)
        {
            Instantiate(shieldBreakEffect, transform.position, Quaternion.identity);
        }
        AudioManager.Instance.PlaySFX(shieldBreakClip);
    }
}
