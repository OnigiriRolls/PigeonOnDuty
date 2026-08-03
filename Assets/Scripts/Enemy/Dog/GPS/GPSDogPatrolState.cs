using UnityEngine;

public class GPSDogPatrolState : GPSDogState
{
    private Vector3 currentTarget;

    public GPSDogPatrolState(GPSDogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
        PickNextPoint();
    }

    public override void UpdateState()
    {
        dog.MoveTowards(currentTarget, dog.PatrolSpeed);
        if (dog.HasReachedDestination())
            PickNextPoint();
        if (dog.PatrolZone.CurrentHuman != null)
        {
            dog.ChangeState(dog.ChaseState);
        }
    }

    private void PickNextPoint()
    {
        currentTarget = dog.PatrolZone.GetRandomPoint();
    }
}
