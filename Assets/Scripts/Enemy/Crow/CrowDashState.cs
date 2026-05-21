using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class CrowDashState : CrowState
{
    private Vector3 dashTarget;

    public CrowDashState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        Vector3 predictedPosition = crow.GetPredictedPlayerPosition(0.8f);
        Vector3 direction = (predictedPosition - crow.transform.position).normalized;
        dashTarget = crow.transform.position + direction * crow.dashDistance;
    }

    public override void UpdateState()
    {
        crow.MoveTowards(dashTarget, crow.dashSpeed);
        float distance = Vector3.Distance(crow.transform.position, dashTarget);
        if (distance < 0.2f)
        {
            Debug.Log("attack");
            crow.currentAttacks++;
            if (crow.currentAttacks >= crow.maxAttacks)
            {
                crow.DestroyCrow();
                //crow.ChangeState(new CrowRecoverState(crow));
               // crow.ChangeState(new CrowChaseState(crow));
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
