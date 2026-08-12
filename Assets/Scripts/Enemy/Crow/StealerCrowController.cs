using UnityEngine;

public class StealerCrowController : BaseCrowController, IThrowTarget, IProjectileTarget
{
    public Transform AimPoint => transform;
    public PatrolZone PatrolZone { get; protected set; }
    public bool HasStolenItem => carriedItem != null;

    [SerializeField] private CarryVisual carryVisual;
    [SerializeField] private GameObject targetRing;
    [SerializeField] private GameObject confusedIcon;

    private ThrowableData carriedItem;
    private Transform despawnPoint;

    public void Initialize(Transform player, PatrolZone patrolZone, Transform despawnPoint)
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

    public override void MoveTowardsDespawn(float speed)
    {
        MoveTowards(despawnPoint.position, speed);
    }

    public override bool HasReachedDespawn()
    {
        return Vector3.Distance(transform.position, despawnPoint.position) < 5f;
    }

    public override void TeleportToPatrol()
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

    public bool OnHit(ThrowableData item)
    {
        if (item.itemName != "Feather")
            return false;
        if (!HasStolenItem)
            return false;
        DropStolenItem();
        ChangeState(ConfusedState);
        return true;
    }

    public void ShowTargetRing(bool show)
    {
        if (targetRing != null)
            targetRing.SetActive(show);
    }

    public override void ShowConfused(bool show)
    {
        if (confusedIcon != null)
            confusedIcon.SetActive(show);
    }

    public bool CanBeHitBy(ThrowableData item)
    {
        return item.itemName == "Feather";
    }

    public override bool CanStartChase()
    {
        return !HasStolenItem && PatrolZone.IsPlayerInside;
    }

    public override Vector3 PickNextPoint()
    {
        return PatrolZone.GetRandomPoint();
    }
}
