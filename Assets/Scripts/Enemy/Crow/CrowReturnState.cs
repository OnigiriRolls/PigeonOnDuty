public class CrowReturnState : CrowState
{
    public CrowReturnState(StealerCrowController crow) : base(crow)
    {
    }

    public override void Enter()
    {
        EnemyAggroManager.Instance.Release(crow);
    }

    public override void UpdateState()
    {
        ((StealerCrowController)crow).MoveTowardsDespawn(crow.Config.chaseSpeed);
        if (!((StealerCrowController)crow).HasReachedDespawn())
            return;
        ((StealerCrowController)crow).TeleportToPatrol();
    }
}
