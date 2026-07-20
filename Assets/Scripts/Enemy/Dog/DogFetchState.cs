using System.Diagnostics;

public class DogFetchState : DogState
{
    private NewspaperProjectile target;

    public DogFetchState(DogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
        dog.ShowInterest();
    }

    public void SetTarget(NewspaperProjectile newspaper)
    {
        target = newspaper;
    }

    public override void UpdateState()
    {
        if (target == null || !target.IsPickup)
        {
            dog.ChangeState(dog.PatrolState);
            return;
        }
        dog.MoveTowards(target.transform.position, dog.Config.patrolSpeed);
    }

    public override void Exit()
    {
        dog.HideInterest();
    }
}
