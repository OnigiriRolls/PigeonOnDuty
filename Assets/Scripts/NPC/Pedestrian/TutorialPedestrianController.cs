using UnityEngine;

[RequireComponent(typeof(ClientController))]
[RequireComponent(typeof(Animator))]
public class TutorialPedestrianController : MonoBehaviour, IThrowTarget, IProjectileTarget, IBasePedestrianController
{
    public Transform AimPoint => transform;
    public ThrowableData CarriedItem { get; private set; }

    [SerializeField] private GameObject targetRing;

    private ClientController clientController;
    private Animator animator;

    private void Awake()
    {
        clientController = GetComponent<ClientController>();
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed", 0f);
    }

    public bool CanBeHitBy(ThrowableData item)
    {
        return item.itemName == "Newspaper";
    }

    public bool OnHit(ThrowableData item)
    {
        if (CarriedItem == null && item.itemName == "Newspaper")
        {
            if (!clientController.IsActiveClient)
                return false;
            PickUp(item.pickupPrefab);
            return true;
        }
        return false;
    }

    public void PickUp(ThrowablePickup pickup)
    {
        CarriedItem = pickup.Item;
        clientController.HandlePickup(pickup.Item);
    }

    public void ShowTargetRing(bool show)
    {
        if (targetRing != null)
            targetRing.SetActive(show);
    }

    public void TryCollectPickup(ThrowablePickup pickup)
    {
    }
}
