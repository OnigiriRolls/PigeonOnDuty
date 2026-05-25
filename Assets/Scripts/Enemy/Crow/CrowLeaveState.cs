public class CrowLeaveState : CrowState
{
    public CrowLeaveState(CrowController crowController) : base(crowController)
    {
    }

    public override void UpdateState()
    {
        crow.MoveTowardsSpawnPositionAndDestroyCrow(crow.Config.chaseSpeed);
    }
}
