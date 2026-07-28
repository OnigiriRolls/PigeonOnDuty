using System;
using System.Collections;
using UnityEngine;

public class ThrowablePickup : MonoBehaviour
{
    public ThrowableData Item => item;
    public bool CanBeCollected => canBeCollected;
    public int Amount => amount;
    public bool IsCollected { get; private set; }
    public bool IsReserved { get; private set; }
    public event Action OnCollected;

    [SerializeField] private ThrowableData item;
    [SerializeField] private int amount = 1;

    private bool canBeCollected;

    private void Start()
    {
        PickupManager.Instance.Register(this);
    }

    private void OnEnable()
    {
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
        OnCollected?.Invoke();
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

    private void OnDisable()
    {
        if (PickupManager.Instance != null)
            PickupManager.Instance.Unregister(this);
    }
}
