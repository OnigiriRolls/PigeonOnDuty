using UnityEngine;

public class CrowDashState : CrowState
{
    private Vector3 dashTarget;

    public CrowDashState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        Rigidbody2D playerRb = crow.player.GetComponent<Rigidbody2D>();
        Vector3 predictedPosition = crow.player.position + (Vector3)playerRb.linearVelocity * 0.5f;
        Vector3 dir = (predictedPosition - crow.transform.position).normalized;
        dashTarget = crow.transform.position + dir * crow.dashDistance;
    }

    public override void UpdateState()
    {
        crow.MoveTowards(dashTarget, crow.dashSpeed);

        float dist = Vector3.Distance(crow.transform.position, dashTarget);

        if (dist < 0.2f)
        {
            crow.currentAttacks++;
            if (crow.currentAttacks >= crow.maxAttacks)
            {
                crow.ChangeState(new CrowRecoverState(crow));
            }
            else
            {
                crow.ChangeState(new CrowChaseState(crow));
            }
        }
    }

    public override void Exit()
    {
    }
}
