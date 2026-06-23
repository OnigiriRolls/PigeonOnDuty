using UnityEngine;

public class JoinedParadeState : IGPSHumanState
{
    private readonly HumanFollower human;
    private float timer;

    public JoinedParadeState(HumanFollower human)
    {
        this.human = human;
    }

    public void Enter()
    {
        timer = 0f;
        human.ShowMessage("This looks fun!");
    }

    public void Exit()
    {
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= human.ParadeDuration)
        {
            human.ChangeState(new DistractedState(human));
        }
    }

    public void FixedUpdate()
    {
    }
}
