using UnityEngine;

public class FollowingState : IGPSHumanState
{
    private readonly HumanFollower human;

    public FollowingState(HumanFollower human)
    {
        this.human = human;
    }

    public void Enter()
    {
       // human.StateLabel = "DISTRACTION";
        human.StateProgress = 1f;
        human.RefreshDistractionMeter();
    }

    public void Exit()
    {
    }

    public void Update()
    {
        human.StateProgress = human.DistractionPercent;
        human.FollowTarget();
        if (human.IsInParade())
            human.ChangeState(new FollowingParadeState(human));
    }

    public void FixedUpdate()
    {
    }
}
