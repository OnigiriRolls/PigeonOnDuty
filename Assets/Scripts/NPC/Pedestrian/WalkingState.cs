public class WalkingState : IPedestrianState
{
    private readonly PedestrianController controller;

    public WalkingState(PedestrianController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.Movement.MoveToNextWaypoint();
    }

    public void Update()
    {
        if (!controller.Movement.HasReachedDestination(3f))
            return;
        controller.ChangeState(controller.WaitingState);
    }

    public void Exit()
    {
    }
}
