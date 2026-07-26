using UnityEngine;

public class DogCarryState : DogState
{
    private Vector3 targetPoint;

    public DogCarryState(DogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
        dog.ShowNewspaper();
        PickNextPoint();
    }

    public override void UpdateState()
    {
        dog.MoveTowards(targetPoint, dog.Config.patrolSpeed);
        if (dog.HasReachedDestination())
        {
            PickNextPoint();
        }
    }

    private void PickNextPoint()
    {
        targetPoint = dog.PatrolZone.GetRandomPoint();
    }

    public override void Exit()
    {
        dog.HideCarryVisual();
    }
}
