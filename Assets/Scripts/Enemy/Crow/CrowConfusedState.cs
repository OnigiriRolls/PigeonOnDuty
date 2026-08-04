using UnityEngine;

public class CrowConfusedState : CrowState
{
    private float timer;

    public CrowConfusedState(BaseCrowController crow) : base(crow)
    {
    }

    public override void Enter()
    {
        timer = crow.Config.confusedDuration;
        crow.ShowConfused(true);
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            crow.ChangeState(crow.PatrolState);
    }

    public override void Exit()
    {
        crow.ShowConfused(false);
    }
}
