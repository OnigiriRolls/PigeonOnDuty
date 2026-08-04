public class CrowReturnState : CrowState
{
    public CrowReturnState(BaseCrowController crow) : base(crow)
    {
    }

    public override void Enter()
    {
        EnemyAggroManager.Instance.Release(crow);
    }

    public override void UpdateState()
    {
        crow.MoveTowardsDespawn(crow.Config.chaseSpeed);
        if (!crow.HasReachedDespawn())
            return;
        crow.TeleportToPatrol();
    }
}
