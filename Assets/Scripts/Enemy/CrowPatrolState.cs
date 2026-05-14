using UnityEngine;

public class CrowPatrolState : CrowState
{
    private Vector3 targetPoint;
    private float waitTimer;
    private bool waiting;

    public CrowPatrolState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        PickNewPoint();
    }

    public override void UpdateState()
    {
        if (crow.PlayerInRange())
        {
            //crow.ChangeState(new CrowChaseState(crow));
            return;
        }

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                waiting = false;
                PickNewPoint();
            }
            return;
        }

        crow.MoveTowards(targetPoint, crow.patrolSpeed);

        float dist = Vector3.Distance(crow.transform.position, targetPoint);
        if (dist < 0.2f)
        {
            waiting = true;
            waitTimer = Random.Range(1f, 3f);
        }
    }

    private void PickNewPoint()
    {
        targetPoint = crow.GetRandomPatrolPoint();
    }
}
