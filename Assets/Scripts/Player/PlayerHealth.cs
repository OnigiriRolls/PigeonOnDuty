using System;
using UnityEngine;

[RequireComponent(typeof(PlayerAudioController))]
public class PlayerHealth : MonoBehaviour
{
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public int CurrentHealth { get; private set; }
    public GameObject HitParticlesPrefab => hitParticlesPrefab;

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject hitParticlesPrefab;

    private PlayerAudioController audioController;
    private GameManager gameManager;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Start()
    {
        audioController = GetComponent<PlayerAudioController>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void TakeDamage(int amount)
    {
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
}
