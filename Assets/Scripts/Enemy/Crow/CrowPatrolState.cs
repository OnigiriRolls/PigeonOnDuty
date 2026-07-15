using UnityEngine;

public class CrowPatrolState : CrowState
{
    private Vector3 currentTarget;

    public CrowPatrolState(StealerCrowController crow) : base(crow)
    {
    }

    public override void Enter()
    {
        PickNextPoint();
    }

    public override void UpdateState()
    {
        crow.MoveTowards(currentTarget, crow.Config.patrolSpeed);
        if (Vector3.Distance(crow.transform.position, currentTarget) < 3f)
            PickNextPoint();

        if (((StealerCrowController)crow).HasStolenItem)
            return;

        float distanceToPlayer = Vector3.Distance(crow.transform.position, crow.Player.position);
        if (distanceToPlayer <= crow.Config.detectionRadius)
        {
            if (EnemyAggroManager.Instance.TryAcquire(crow))
                crow.ChangeState(crow.ChaseState);
        }
    }

    private void PickNextPoint()
    {
        currentTarget = crow.PatrolZone.GetRandomPoint();
    }
}
