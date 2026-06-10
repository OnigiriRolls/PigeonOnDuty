using System;
using System.Collections;
using TMPro;
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
    [SerializeField] private float shieldInvulnerabilityDuration = 2f;
    [SerializeField] private GameObject hitParticlesPrefab;
    [SerializeField] private GameObject shield;
    [SerializeField] private ParticleSystem shieldBreakEffect;
    [SerializeField] private AudioClip shieldBreakClip;
    [SerializeField] private float shieldDuration = 10f;
    [SerializeField] private float pulseStartTime = 3f;
    [SerializeField] private Renderer birdRenderer;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject deathReasonText;
    [SerializeField] private GameObject GameOverUI;

    private static readonly WaitForSeconds WAIT_FOR_SECONDS_0_1 = new(0.1f);

    private Coroutine shieldCoroutine;
    private PlayerAudioController audioController;
    private GameManager gameManager;
    private DeliveryMissionManager deliveryMissionManager;
    private Animator shieldAnimator;
    private Material birdMaterial;
    private bool isInvulnerable;
    private Coroutine pulseCoroutine;
    private Coroutine invulnerabilityCoroutine;
    private DeathReason lastDamageReason;
    private bool dead;
    private GameOverUI gameOverUI;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);
        birdMaterial = new Material(birdRenderer.material);
        birdRenderer.material = birdMaterial;
    }

    private void Start()
    {
        audioController = GetComponent<PlayerAudioController>();
        gameManager = FindAnyObjectByType<GameManager>();
        shieldAnimator = shield.GetComponent<Animator>();
        deliveryMissionManager = FindAnyObjectByType<DeliveryMissionManager>();
        gameOverUI = GameOverUI.GetComponent<GameOverUI>();
        dead = false;
    }

    public void TakeDamage(int amount, DeathReason damageReason)
    {
        if (isInvulnerable)
            return;

        if (HasShield)
        {
            ConsumeShield(true);
            return;
        }

        lastDamageReason = damageReason;
        audioController.PlayHitClip();
        DeliveryMission mission = deliveryMissionManager.ActiveMission;
        if (mission != null && mission.oneHitFail)
        {
            CurrentHealth = 0;
            OnHealthChanged?.Invoke(0);
            Die();
            return;
        }
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
        if (CurrentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Die()
    {
        if (dead)
            return;
        dead = true;
        //Debug.Log("GAME OVER");
        OnDeath?.Invoke();
        deathReasonText.GetComponent<TextMeshProUGUI>().text = gameOverUI.GetDeathReasonText(lastDamageReason);
        deathReasonText.SetActive(true);
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        enabled = false;
        yield return new WaitForSeconds(3f);
        gameManager.GameOver(lastDamageReason);
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
        if (blockedDamage)
            StartInvulnerability(shieldInvulnerabilityDuration);
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

    private IEnumerator InvulnerabilityRoutine(float duration)
    {
        isInvulnerable = true;
        playerAnimator.SetBool("Pulse", true);
        pulseCoroutine = StartCoroutine(PulseRoutine());
        yield return new WaitForSeconds(duration);
        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);
        playerAnimator.SetBool("Pulse", false);
        birdMaterial.SetColor("_EmissionColor", Color.black);
        isInvulnerable = false;
    }

    public void StartInvulnerability(float duration)
    {
        if (HasShield)
            return;
        if (invulnerabilityCoroutine != null)
            StopCoroutine(invulnerabilityCoroutine);
        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityRoutine(duration));
    }

    private IEnumerator PulseRoutine()
    {
        while (true)
        {
            float emission = 2f + Mathf.PingPong(Time.time * 8f, 3f);
            Color emissionColor = Color.white * emission;
            birdMaterial.SetColor("_EmissionColor", emissionColor);
            yield return WAIT_FOR_SECONDS_0_1;
        }
    }
}
