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
        Debug.Log("Entering Following State");
    }

    public void Exit()
    {
    }

    public void Update()
    {
        human.StateProgress = human.DistractionPercent;
    }

    public void FixedUpdate()
    {
        human.FollowTarget();
        if (human.IsDistracted())
            human.ChangeState(new DistractedState(human));
    }
}
