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

    public void MoveTowardsDespawn(float speed)
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
}
