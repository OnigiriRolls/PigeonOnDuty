using UnityEngine;

public class DogCaughtState : IGPSHumanState
{
    private readonly HumanFollower human;

    public DogCaughtState(HumanFollower human)
    {
        this.human = human;
    }

    public void Enter()
    {
        human.StopAgent();
        human.ShowMessage("Help! That dog is scary!");
        human.ShowHint("Scare the dog away with a feather!");
        human.ShowInteractionCircle(Color.red);
    }

    public void Exit()
    {
        human.HideHint();
        human.HideInteractionCircle();
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }
}
