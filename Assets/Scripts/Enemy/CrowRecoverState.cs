using UnityEngine;

public class CrowRecoverState : CrowState
{
    private float timer;

    public CrowRecoverState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        timer = crow.recoverTime;
        crow.currentAttacks = 0;
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            crow.ChangeState(new CrowLeaveState(crow));
        }
    }

    public override void Exit()
    {
    }
}
