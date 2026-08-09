using System;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.STP;

[RequireComponent(typeof(Animator))]
public class TutorialDogController : MonoBehaviour, IThrowTarget, IProjectileTarget
{
    public Transform AimPoint => transform;
    public event Action OnScared;

    [SerializeField] private DogConfig config;
    [SerializeField] private CarryVisual carryVisual;
    [SerializeField] private GameObject targetRing;

    private Animator animator;
    private bool hasNewspaper;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed", 0f);
    }

    public void SetupTutorialNewspaper()
    {
        hasNewspaper = true;
        carryVisual.Show(config.newspaper);
    }

    public bool CanBeHitBy(ThrowableData item)
    {
        return item.itemName == "Feather";
    }

    public bool OnHit(ThrowableData item)
    {
        if (item.itemName != "Feather")
            return false;
        DropNewspaper();
        OnScared?.Invoke();
        return true;
    }

    public void ShowTargetRing(bool show)
    {
        targetRing.SetActive(show);
    }

    public void DropNewspaper()
    {
        if (!hasNewspaper)
            return;
        hasNewspaper = false;
        Instantiate(config.newspaper.pickupPrefab, transform.position, Quaternion.identity);
    }
}
