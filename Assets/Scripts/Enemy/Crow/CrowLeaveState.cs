public class CrowLeaveState : CrowState
{
    public CrowLeaveState(AttackCrowController crowController) : base(crowController)
    {
        crow = crowController;
    }

    public override void Enter()
    {
        EnemyAggroManager.Instance.Release(crow);
    }

    public override void UpdateState()
    {
        ((AttackCrowController)crow).MoveTowardsDespawn(crow.Config.chaseSpeed);
    }
}
