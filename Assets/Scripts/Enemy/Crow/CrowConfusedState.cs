using UnityEngine;

public class CrowConfusedState : CrowState
{
    private float timer;

    public CrowConfusedState(StealerCrowController crow) : base(crow)
    {
    }

    public override void Enter()
    {
        timer = crow.Config.confusedDuration;
        ((StealerCrowController)crow).ShowConfused(true);
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            crow.ChangeState(((StealerCrowController)crow).PatrolState);
    }

    public override void Exit()
    {
        ((StealerCrowController)crow).ShowConfused(false);
    }
}
