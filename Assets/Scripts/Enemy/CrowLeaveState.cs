using UnityEngine;

public class CrowLeaveState : CrowState
{
    private float destroyTimer = 10f;

    public CrowLeaveState(CrowController crowController) : base(crowController)
    {
    }

    public override void UpdateState()
    {
        crow.MoveTowards(
                    crow.patrolCenter,
                    crow.chaseSpeed
                );

        destroyTimer -= Time.deltaTime;

        if (destroyTimer <= 0)
        {
            GameObject.Destroy(crow.gameObject);
        }
    }
}
