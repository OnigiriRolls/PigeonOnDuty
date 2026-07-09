using UnityEngine;

[RequireComponent(typeof(PedestrianMovement))]
public class PedestrianController : MonoBehaviour
{
    public PedestrianMovement Movement => movement;
    public WaypointNetwork WaypointNetwork { get; private set; }
    public ThrowableData CarriedItem { get; private set; }
    public WalkingState WalkingState => walkingState;
    public WaitingState WaitingState => waitingState;
    public CollectPickupState CollectPickupState => collectPickupState;
    public bool IsWalking => currentState == walkingState;
    public bool IsWaiting => currentState == waitingState;

    [field: SerializeField] public float MinWaitTime { get; private set; } = 1f;
    [field: SerializeField] public float MaxWaitTime { get; private set; } = 4f;

    [SerializeField] private PedestrianCarryVisual carryVisual;

    private PedestrianMovement movement;
    private IPedestrianState currentState;
    private WalkingState walkingState;
    private WaitingState waitingState;
    private CollectPickupState collectPickupState;

    private void Awake()
    {
        movement = GetComponent<PedestrianMovement>();
        walkingState = new WalkingState(this);
        waitingState = new WaitingState(this);
        collectPickupState = new CollectPickupState(this);
    }

    public void Initialize(WaypointNetwork network)
    {
        WaypointNetwork = network;
    }

    private void Start()
    {
        ChangeState(walkingState);
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(IPedestrianState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void TryCollectPickup(ThrowablePickup pickup)
    {
        if (pickup == null)
            return;
        if (!pickup.TryReserve())
            return;
        if (currentState is CollectPickupState)
            return;
        collectPickupState.SetPickup(pickup);
        ChangeState(collectPickupState);
    }

    public void PickUp(ThrowablePickup pickup)
    {
        CarriedItem = pickup.Item;
        carryVisual.Show(CarriedItem);
    }

    public void DropItem()
    {
        CarriedItem = null;
        carryVisual.Hide();
    }
}
