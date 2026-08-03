using UnityEngine;

public class GPSDogScaredState : GPSDogState
{
    private float timer;
    private const float ScaredDuration = 10f;

    public GPSDogScaredState(GPSDogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
        timer = 0f;
        dog.PlayScaredSound();
        HumanFollower human = dog.PatrolZone.CurrentHuman;
        if (human != null)
            human.OnDogGone();
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;
        if (timer >= ScaredDuration)
            dog.ChangeState(dog.PatrolState);
    }
}
