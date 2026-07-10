using UnityEngine;

public class CarryItemState : IPedestrianState
{
    private readonly PedestrianController controller;
    private float timer;

    public CarryItemState(PedestrianController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.Movement.MoveToNextWaypoint();
        timer = controller.CarryDuration;
    }

    public void Exit()
    {
    }

    public void Update()
    {
        timer -= Time.deltaTime;

        if (timer > 0f)
            return;

        controller.DropCarriedItem();
        controller.ChangeState(controller.WalkingState);
    }
}
