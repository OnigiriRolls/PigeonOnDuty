using UnityEngine;

public class ReturningState : IGPSHumanState
{
    private readonly HumanFollower human;

    public ReturningState(HumanFollower human)
    {
        this.human = human;
    }

    public void Enter()
    {
        human.ShowMessage("Let's keep going!");
        human.HideInteractionCircle();
        human.HideHint();
    }

    public void Exit()
    {
    }

    public void Update()
    {
        human.FollowTarget();
        if (!human.IsNearParade())
        {
            Debug.Log("stop return state");
            human.ChangeState(new FollowingState(human));
        }
    }

    public void FixedUpdate()
    {
    }
}
