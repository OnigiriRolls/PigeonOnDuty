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
        if (Vector3.Distance(dog.transform.position, currentTarget) < 1f)
            PickNextPoint();
    }

    private void PickNextPoint()
    {
        currentTarget = dog.PatrolZone.GetRandomPoint();
    }
}
