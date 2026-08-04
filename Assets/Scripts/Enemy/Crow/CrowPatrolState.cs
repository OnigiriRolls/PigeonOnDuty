using UnityEngine;

public class CrowPatrolState : CrowState
{
    private Vector3 currentTarget;

    public CrowPatrolState(BaseCrowController crow) : base(crow)
    {
    }

    public override void Enter()
    {
        currentTarget = crow.PickNextPoint();
    }

    public override void UpdateState()
    {
        crow.MoveTowards(currentTarget, crow.Config.patrolSpeed);
        if (Vector3.Distance(crow.transform.position, currentTarget) < 3f)
            currentTarget = crow.PickNextPoint();
        if (crow.CanStartChase())
        {
            if (EnemyAggroManager.Instance.TryAcquire(crow))
                crow.ChangeState(crow.ChaseState);
        }
    }
}
