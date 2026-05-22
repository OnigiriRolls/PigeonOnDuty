using UnityEngine;

public class CrowChaseState : CrowState
{
    private float waitTimer;
    private bool reachedPigeon;

    public CrowChaseState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        crow.CanHitPlayer = false;
        waitTimer = Random.Range(crow.minWaitTime, crow.maxWaitTime);
        reachedPigeon = false;
    }

    public override void UpdateState()
    {
        float distance = Vector3.Distance(crow.transform.position, crow.player.position);
        Vector3 predictedPosition = crow.GetPredictedPlayerPositionWithOffset(0.7f);
        crow.MoveTowards(predictedPosition, crow.chaseSpeed);
        if (distance < crow.followTolerance)
        {
            reachedPigeon = true;
        }

        if (reachedPigeon)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                crow.ChangeState(new CrowAttackState(crow));
            }
        }
    }
}
