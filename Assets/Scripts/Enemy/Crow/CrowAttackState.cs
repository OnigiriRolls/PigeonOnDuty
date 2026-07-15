using UnityEngine;

public class CrowAttackState : CrowState
{
    private Vector3 dashTarget;
    private bool dashStarted;

    public CrowAttackState(BaseCrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        crow.CanHitPlayer = true;
        dashStarted = false;
        dashTarget = crow.Player.position;
    }

    public override void UpdateState()
    {
        if (!dashStarted)
            dashStarted = true;

        crow.MoveTowards(dashTarget, crow.Config.dashSpeed);
        float distanceToTarget = Vector3.Distance(crow.transform.position, dashTarget);
        if (distanceToTarget < 1f)
        {
            crow.CurrentAttacks++;
            if (crow.CurrentAttacks >= crow.Config.maxAttacks)
            {
                crow.OnAttackFinished();
            }
            else
            {
                crow.ChangeState(crow.ChaseState);
            }
        }
    }
}
