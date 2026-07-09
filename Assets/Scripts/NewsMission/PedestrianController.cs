using UnityEngine;

[RequireComponent(typeof(PedestrianMovement))]
public class PedestrianController : MonoBehaviour
{
    public PedestrianMovement Movement => movement;
    public WaypointNetwork WaypointNetwork { get; private set; }
    public WalkingState WalkingState => walkingState;
    public WaitingState WaitingState => waitingState;
    public bool IsWalking => currentState == walkingState;
    public bool IsWaiting => currentState == waitingState;

    [field: SerializeField] public float MinWaitTime { get; private set; } = 1f;
    [field: SerializeField] public float MaxWaitTime { get; private set; } = 4f;

    private PedestrianMovement movement;
    private IPedestrianState currentState;
    private WalkingState walkingState;
    private WaitingState waitingState;

    private void Awake()
    {
        movement = GetComponent<PedestrianMovement>();
        walkingState = new WalkingState(this);
        waitingState = new WaitingState(this);
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
        ChangeState(new CollectPickupState(this, pickup));
    }
}
