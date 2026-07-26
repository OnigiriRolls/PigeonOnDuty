using System.Collections;
using UnityEngine;

public class ThrowablePickup : MonoBehaviour
{
    public ThrowableData Item => item;
    public bool CanBeCollected => canBeCollected;
    public bool IsCollected { get; private set; }
    public bool IsReserved { get; private set; }

    [SerializeField] private ThrowableData item;
    [SerializeField] private Animator animator;
    [SerializeField] private float pulseDuration = 3f;
    [SerializeField] private GameObject colliderZone;

    private bool canBeCollected;

    private void Start()
    {
        //StartCoroutine(LifetimeRoutine());
        colliderZone.SetActive(true);
    }

    private void OnEnable()
    {
        PickupManager.Instance.Register(this);
        StartCoroutine(EnablePickup());
    }

    private IEnumerator EnablePickup()
    {
        canBeCollected = false;
        yield return new WaitForSeconds(0.5f);
        canBeCollected = true;
    }

    public void Collect()
    {
        if (IsCollected)
            return;
        IsCollected = true;
        Destroy(gameObject);
    }

    public bool TryReserve()
    {
        if (IsReserved)
            return false;

        IsReserved = true;
        return true;
    }

    public void ReleaseReservation()
    {
        IsReserved = false;
    }

    private IEnumerator LifetimeRoutine()
    {
        float waitTime = Mathf.Max(0f, item.pickupLifetime - pulseDuration);
        yield return new WaitForSeconds(waitTime);
        if (animator != null)
            animator.SetTrigger("Pulse");
        yield return new WaitForSeconds(pulseDuration);
        if (!IsCollected)
            Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (PickupManager.Instance != null)
            PickupManager.Instance.Unregister(this);
    }
}
