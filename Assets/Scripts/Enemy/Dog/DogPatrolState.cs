using UnityEngine;

public class DogPatrolState : DogState
{
    private Vector3 currentTarget;

    public DogPatrolState(DogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
        PickNextPoint();
    }

    public override void UpdateState()
    {
        dog.MoveTowards(currentTarget, dog.Config.patrolSpeed);
        if (dog.HasReachedDestination())
            PickNextPoint();
        ThrowablePickup pickup = PickupManager.Instance.GetClosestPickup(dog.PatrolZone, dog.Config.newspaper);
        if (pickup != null)
        {
            dog.FetchState.SetTarget(pickup);
            dog.ChangeState(dog.FetchState);
        }
    }

    private void PickNextPoint()
    {
        currentTarget = dog.PatrolZone.GetRandomPoint();
    }
}
