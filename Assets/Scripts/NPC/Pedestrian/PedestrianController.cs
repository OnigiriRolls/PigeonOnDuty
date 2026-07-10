using UnityEngine;
[RequireComponent(typeof(PedestrianMovement))]

[RequireComponent(typeof(ClientController))]
public class PedestrianController : MonoBehaviour
{
    public PedestrianMovement Movement => movement;
    public WaypointNetwork WaypointNetwork { get; private set; }
    public ThrowableData CarriedItem { get; private set; }
    public WalkingState WalkingState => walkingState;
    public WaitingState WaitingState => waitingState;
    public CollectPickupState CollectPickupState => collectPickupState;
    public CarryItemState CarryItemState => carryItemState;
    public bool IsWalking => currentState == walkingState;
    public bool IsWaiting => currentState == waitingState;
    public float CarryDuration => carryDuration;

    [field: SerializeField] public float MinWaitTime { get; private set; } = 1f;
    [field: SerializeField] public float MaxWaitTime { get; private set; } = 4f;

    [SerializeField] private PedestrianCarryVisual carryVisual;
    [SerializeField] private float carryDuration = 10f;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private string currentStateName;
    [SerializeField] private float pickupCooldownDuration = 2f;

    private PedestrianMovement movement;
    private IPedestrianState currentState;
    private WalkingState walkingState;
    private WaitingState waitingState;
    private CollectPickupState collectPickupState;
    private CarryItemState carryItemState;
    private ClientController clientController;
    private float pickupCooldown;

    private void Awake()
    {
        movement = GetComponent<PedestrianMovement>();
        clientController = GetComponent<ClientController>();
        walkingState = new WalkingState(this);
        waitingState = new WaitingState(this);
        collectPickupState = new CollectPickupState(this);
        carryItemState = new CarryItemState(this);
    }

    public void Initialize(WaypointNetwork network)
    {
        WaypointNetwork = network;
        movement.Initialize(network);
    }

    private void Start()
    {
        ChangeState(walkingState);
    }

    private void Update()
    {
        if (pickupCooldown > 0f)
            pickupCooldown -= Time.deltaTime;

        currentState?.Update();
    }

    public void ChangeState(IPedestrianState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentStateName = currentState.GetType().Name;
        currentState.Enter();
    }

    public void TryCollectPickup(ThrowablePickup pickup)
    {
        if (pickupCooldown > 0f)
            return;
        if (CarriedItem != null)
            return;
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
        clientController.HandlePickup(pickup.Item);
        clientController.ClearClient();
    }

    public void DropCarriedItem()
    {
        if (CarriedItem == null)
            return;
        var pickup = Instantiate(CarriedItem.pickupPrefab, dropPoint.position, Quaternion.identity);
        carryVisual.Hide();
        CarriedItem = null;
        pickupCooldown = pickupCooldownDuration;
    }
}
