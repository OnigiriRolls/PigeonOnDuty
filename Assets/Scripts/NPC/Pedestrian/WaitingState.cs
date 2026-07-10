using UnityEngine;

public class WaitingState : IPedestrianState
{
    private readonly PedestrianController controller;
    private float timer;

    public WaitingState(PedestrianController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        timer = Random.Range(controller.MinWaitTime, controller.MaxWaitTime);
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;
        controller.ChangeState(controller.WalkingState);
    }

    public void Exit()
    {
    }
}
