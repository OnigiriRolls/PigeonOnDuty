using NUnit.Framework.Constraints;
using UnityEngine;

public class CrowChaseState : CrowState
{
    private float stayBehindTimer;
    private bool reachedBehindPoint;

    public CrowChaseState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        stayBehindTimer = Random.Range(2f, 4f);
        reachedBehindPoint = false;
    }

    public override void UpdateState()
    {
        Vector3 playerBehindPosition = crow.player.position - crow.player.right * crow.followDistance;
        float dist = Vector3.Distance(crow.transform.position, playerBehindPosition);
        if (dist > crow.tolerance)
        {
            crow.MoveTowards(playerBehindPosition, crow.chaseSpeed);
        }

        //Debug.Log(reachedBehindPoint);
        if (dist < 1f)
        {
            reachedBehindPoint = true;
        }

        if (reachedBehindPoint)
        {
            stayBehindTimer -= Time.deltaTime;

            if (stayBehindTimer <= 0)
            {
                // crow.ChangeState(new CrowDashState(crow));
            }
        }
    }
}
