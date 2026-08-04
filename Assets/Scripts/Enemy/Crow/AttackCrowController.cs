using UnityEngine;

public class AttackCrowController : BaseCrowController
{
    public CrowLeaveState LeaveState { get; protected set; }

    private CrowSpawner spawner;
    private Transform despawnPoint;

    protected override void Awake()
    {
        base.Awake();
        LeaveState = new CrowLeaveState(this);
    }

    public void Initialize(Transform player, CrowSpawner spawner, Transform despawnPoint)
    {
        InitializePlayer(player);
        this.spawner = spawner;
        this.despawnPoint = despawnPoint;
        ChangeState(ChaseState);
    }

    public override void OnPlayerHit()
    {
        config.ability.Execute(playerController);
    }

    public override void OnAttackFinished()
    {
        ChangeState(LeaveState);
    }

    public override void MoveTowardsDespawn(float speed)
    {
        MoveTowards(despawnPoint.position, speed);
        if (Vector3.Distance(transform.position, despawnPoint.position) < 5f)
        {
            spawner.FinishEnemy();
            Destroy(gameObject);
        }
    }

    public override void DestroyCrow()
    {
        spawner.FinishEnemy();
        base.DestroyCrow();
    }

    public override bool HasReachedDespawn()
    {
        throw new System.NotImplementedException();
    }

    public override void TeleportToPatrol()
    {
        throw new System.NotImplementedException();
    }

    public override bool CanStartChase()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowConfused(bool show)
    {
        throw new System.NotImplementedException();
    }

    public override Vector3 PickNextPoint()
    {
        throw new System.NotImplementedException();
    }
}
