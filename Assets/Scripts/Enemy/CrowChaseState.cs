using UnityEngine;

public class CrowChaseState : CrowState
{
    private float stayBehindTimer;
    private bool reachedBehindPoint;
    private bool orbiting;

    private float orbitAngle;
    private float orbitRadius = 6f;
    private float orbitHeight = 4f;
    private float orbitSpeed = 2f;
    private float predictionTime = 0.5f;
    private int targetOrbits;
    private float orbitEnterDistance = 6f;
    private float completedOrbits;

    public CrowChaseState(CrowController crowController) : base(crowController)
    {
    }

    public override void Enter()
    {
        orbiting = false;
        orbitAngle = 0f;
        completedOrbits = 0f;
        targetOrbits = Random.Range(2, 5);
        crow.SetMoveSpeed(crow.chaseSpeed);
        stayBehindTimer = Random.Range(2f, 4f);
        reachedBehindPoint = false;
    }

    public override void UpdateState()
    {
        //Vector3 playerBehindPosition = crow.player.position - crow.player.right * crow.followDistance;
        //Vector3 toPlayer =
        //    crow.player.position
        //    - crow.transform.position * crow.followDistance;

        //float dist = playerBehindPosition.magnitude;

        //crow.UpdateDelayedTarget(
        //    playerBehindPosition
        //);

        Rigidbody playerRb = crow.player.GetComponent<Rigidbody>();
        Vector3 predictedPosition = crow.player.position + playerRb.linearVelocity * predictionTime;
        orbitAngle += orbitSpeed * Time.deltaTime;
        Vector3 orbitOffset = new Vector3(Mathf.Cos(orbitAngle), 0, Mathf.Sin(orbitAngle)) * orbitRadius;
        Vector3 heightOffset = Vector3.up * orbitHeight;
        Vector3 orbitTarget = predictedPosition + orbitOffset + heightOffset;
        crow.MoveSmoothlyTo(orbitTarget, crow.chaseSpeed);
        completedOrbits = orbitAngle / (Mathf.PI * 2f);

        if (completedOrbits >= targetOrbits)
        {
            //crow.ChangeState(
            //    new CrowDashState(crow)
            //);
        }
    }
}
