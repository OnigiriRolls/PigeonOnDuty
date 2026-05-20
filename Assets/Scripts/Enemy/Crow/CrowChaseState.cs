using Unity.VisualScripting;
using UnityEngine;

public class CrowChaseState : CrowState
{
    private float stayBehindTimer;
    private bool reachedBehindPoint;
    private float pauseTimer;
    private bool isWaiting;

    public CrowChaseState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        stayBehindTimer = Random.Range(2f, 4f);
        reachedBehindPoint = false;
        pauseTimer = 0f;
        isWaiting = false;
    }

    public override void UpdateState()
    {
        float distance = Vector3.Distance(crow.transform.position, crow.player.position);
        if (distance <= crow.followDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                pauseTimer = crow.followPauseDuration;
            }
            pauseTimer -= Time.deltaTime;
            if (pauseTimer > 0f)
            {
                return;
            }
            isWaiting = false;
        }

        if (!isWaiting)
        {
            Vector3 predictedPosition = crow.GetPredictedPlayerPosition(0.7f);
            crow.MoveTowards(predictedPosition, crow.chaseSpeed);
        }
        else
        {
            Vector3 predictedPosition = crow.GetPredictedPlayerPosition(0.7f);
            crow.MoveTowards(predictedPosition, 0f);
        }

        //Debug.Log(reachedBehindPoint);
        //if (dist < 1f)
        //{
        //    reachedBehindPoint = true;
        //}

        //if (reachedBehindPoint)
        //{
        //    stayBehindTimer -= Time.deltaTime;

        //    if (stayBehindTimer <= 0)
        //    {
        //        // crow.ChangeState(new CrowDashState(crow));
        //    }
        //}
    }
}
