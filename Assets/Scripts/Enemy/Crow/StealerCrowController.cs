using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class StealerCrowController : BaseCrowController, IThrowTarget
{
    public CrowPatrolZone PatrolZone { get; protected set; }
    public bool HasStolenItem => carriedItem != null;
    public CrowReturnState ReturnState { get; private set; }
    public CrowPatrolState PatrolState { get; protected set; }
    public CrowConfusedState ConfusedState { get; protected set; }

    [SerializeField] private CarryVisual carryVisual;
    [SerializeField] private GameObject targetRing;
    [SerializeField] private GameObject confusedIcon;

    private ThrowableData carriedItem;
    private Transform despawnPoint;

    protected override void Awake()
    {
        base.Awake();
        PatrolState = new CrowPatrolState(this);
        ReturnState = new CrowReturnState(this);
        ConfusedState = new CrowConfusedState(this);
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

        ThrowableProjectile projectile = Instantiate(carriedItem.projectilePrefab, transform.position, Quaternion.identity);
        Vector3 velocity = transform.forward * 2f + Vector3.down * 4f;
        projectile.Launch(velocity);
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

    public void OnHit(ThrowableData item)
    {
        if (item.itemName != "Feather")
            return;
        if (!HasStolenItem)
            return;
        DropStolenItem();
        ChangeState(ConfusedState);
    }

    public void ShowTargetRing(bool show)
    {
        targetRing.SetActive(show);
    }

    public void ShowConfused(bool show)
    {
        if (confusedIcon != null)
            confusedIcon.SetActive(show);
    }
}
