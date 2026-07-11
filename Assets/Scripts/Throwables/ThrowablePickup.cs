using System.Collections;
using UnityEngine;

public class ThrowablePickup : MonoBehaviour
{
    public ThrowableData Item => item;
    public bool IsCollected { get; private set; }
    public bool IsReserved { get; private set; }

    [SerializeField] private ThrowableData item;
    [SerializeField] private Animator animator;
    [SerializeField] private float pulseDuration = 3f;
    [SerializeField] private GameObject colliderZone;

    private void Start()
    {
        StartCoroutine(LifetimeRoutine());
        colliderZone.SetActive(true);
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
}
