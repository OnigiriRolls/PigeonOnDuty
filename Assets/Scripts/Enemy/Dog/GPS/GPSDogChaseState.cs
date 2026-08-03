using UnityEngine;

public class GPSDogChaseState : GPSDogState
{
    public GPSDogChaseState(GPSDogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
    }

    public override void UpdateState()
    {
        if (dog.PatrolZone.CurrentHuman == null)
        {
            dog.ChangeState(dog.PatrolState);
            return;
        }
        dog.MoveTowards(dog.PatrolZone.CurrentHuman.transform.position, dog.ChaseSpeed);
        if (dog.HasReachedHuman(dog.PatrolZone.CurrentHuman))
        {
            dog.ChangeState(dog.GuardState);
        }
    }
}
