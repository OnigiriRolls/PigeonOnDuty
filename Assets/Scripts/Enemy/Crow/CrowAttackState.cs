using UnityEngine;

public class CrowAttackState : CrowState
{
    private Vector3 dashTarget;
    private bool dashStarted;

    public CrowAttackState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        crow.CanHitPlayer = true;
        dashStarted = false;
        dashTarget = crow.player.position;
    }

    public override void UpdateState()
    {
        if (!dashStarted)
            dashStarted = true;

        crow.MoveTowards(dashTarget, crow.dashSpeed);
        float distanceToTarget = Vector3.Distance(crow.transform.position, dashTarget);
        if (distanceToTarget < 1f)
        {
            crow.currentAttacks++;
            if (crow.currentAttacks >= crow.maxAttacks)
            {
                crow.ChangeState(new CrowLeaveState(crow));
            }
            else
            {
                crow.ChangeState(new CrowChaseState(crow));
            }
        }
    }
}
