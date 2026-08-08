using UnityEngine;

public class GPSDogChaseState : GPSDogState
{
    private float timer;
    private bool humanLeftZone;
    private HumanFollower targetHuman;

    public GPSDogChaseState(GPSDogController dog) : base(dog)
    {
    }

    public override void Enter()
    {
        timer = 0f;
        humanLeftZone = false;
        targetHuman = dog.PatrolZone.CurrentHuman;
        dog.PlayBark();
    }

    public override void UpdateState()
    {
        if (targetHuman == null)
        {
            dog.ChangeState(dog.PatrolState);
            return;
        }

        if (dog.PatrolZone.CurrentHuman == null)
        {
            if (!humanLeftZone)
            {
                humanLeftZone = true;
                timer = 0f;
            }

            timer += Time.deltaTime;
            if (timer >= dog.MaxChaseDuration)
            {
                dog.ChangeState(dog.PatrolState);
                return;
            }
        }

        dog.MoveTowards(targetHuman.transform.position, dog.ChaseSpeed);
        if (dog.HasReachedHuman(targetHuman))
        {
            dog.ChangeState(dog.GuardState);
        }
    }

    public override void Exit()
    {
        targetHuman = null;
        dog.StopSound();
    }
}
