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
        if (dog.PatrolZone.HasNewspapers)
        {
            var newspaper = dog.PatrolZone.GetClosestNewspaper(dog.transform.position);
            if (newspaper != null)
            {
                dog.FetchState.SetTarget(newspaper);
                dog.ChangeState(dog.FetchState);
            }
        }
    }

    private void PickNextPoint()
    {
        currentTarget = dog.PatrolZone.GetRandomPoint();
    }
}
