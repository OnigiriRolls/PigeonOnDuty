using UnityEngine;

public class StealerCrowController : BaseCrowController
{
    public bool HasStolenItem => carriedItem != null;
    public CrowReturnState ReturnState { get; private set; }
    public CrowPatrolState PatrolState { get; protected set; }

    [SerializeField] private CarryVisual carryVisual;

    private ThrowableData carriedItem;
    private Transform despawnPoint;

    protected override void Awake()
    {
        base.Awake();
        PatrolState = new CrowPatrolState(this);
        ReturnState = new CrowReturnState(this);
    }

    public void Initialize(Transform player, CrowPatrolZone patrolZone, Transform despawnPoint)
    {
        InitializePlayer(player);
        PatrolZone = patrolZone;
        this.despawnPoint = despawnPoint;
        ChangeState(PatrolState);
    }

    public override void OnPlayerHit()
    {
        if (HasStolenItem)
            return;
        ThrowableData stolenItem = config.ability.Execute(playerController);
        if (stolenItem == null)
            return;
        carriedItem = stolenItem;
        carryVisual.Show(stolenItem);
    }

    public override void OnAttackFinished()
    {
        ChangeState(ReturnState);
    }

    public void DropStolenItem()
    {
        if (carriedItem == null)
            return;

        Instantiate(carriedItem.pickupPrefab, transform.position, Quaternion.identity);
        carryVisual.Hide();
        carriedItem = null;
    }

    public void MoveTowardsDespawn(float speed)
    {
        MoveTowards(despawnPoint.position, speed);
    }

    public bool HasReachedDespawn()
    {
        return Vector3.Distance(transform.position, despawnPoint.position) < 5f;
    }

    public void TeleportToPatrol()
    {
        transform.position = PatrolZone.Center;
        gameObject.SetActive(true);
        ChangeState(PatrolState);
    }

    public void Escape()
    {
        gameObject.SetActive(false);
        Invoke(nameof(TeleportToPatrol), 2f);
    }
}
