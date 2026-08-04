using UnityEngine;

public class PatrolCrowController : BaseCrowController, IProjectileTarget, IThrowTarget
{
    public PatrolZone PatrolZone { get; protected set; }
    public Transform AimPoint => transform;

    [SerializeField] private GameObject targetRing;
    [SerializeField] private GameObject confusedIcon;

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
        config.ability.Execute(playerController);
    }

    public override void OnAttackFinished()
    {
        ChangeState(ReturnState);
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
        ChangeState(ConfusedState);
        return true;
    }

    public void ShowTargetRing(bool show)
    {
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
        return PatrolZone.IsPlayerInside;
    }

    public override Vector3 PickNextPoint()
    {
        return PatrolZone.GetRandomPoint();
    }
}
