using UnityEngine;

public class GPSDogGuardState : GPSDogState
{
    public GPSDogGuardState(GPSDogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
        dog.StopMoving();
        dog.PlayBark();
    }

    public override void UpdateState()
    {
        HumanFollower human = dog.PatrolZone.CurrentHuman;
        if (human == null)
        {
            dog.ChangeState(dog.PatrolState);
            return;
        }
        human.OnDogScared();
    }

    public override void Exit()
    {
        HumanFollower human = dog.PatrolZone.CurrentHuman;
        if (human != null)
            human.OnDogGone();
    }
}
