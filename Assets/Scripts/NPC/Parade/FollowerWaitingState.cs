using UnityEngine;

public class FollowerWaitingState : IGPSHumanState
{
    private readonly HumanFollower human;

    public FollowerWaitingState(HumanFollower human)
    {
        this.human = human;
    }

    public void Enter()
    {
        human.StopAgent();
        human.ShowMessage("I'll wait here.");
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }
}
