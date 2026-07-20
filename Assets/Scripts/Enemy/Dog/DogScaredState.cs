using UnityEngine;

public class DogScaredState : DogState
{
    private float timer;
    private Vector3 escapePoint;

    public DogScaredState(DogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
        dog.PlayScaredSound();
        timer = dog.Config.scaredDuration;
        dog.DropNewspaper();
        escapePoint = dog.PatrolZone.GetRandomPoint();
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        dog.MoveTowards(escapePoint, dog.Config.scaredSpeed);
        if (timer <= 0f)
            dog.ChangeState(dog.PatrolState);
    }
}
