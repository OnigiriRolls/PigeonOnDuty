public class WalkingState : IPedestrianState
{
    private readonly PedestrianController controller;

    public WalkingState(PedestrianController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        PedestrianWaypoint waypoint = controller.WaypointNetwork.GetRandomWaypoint();
        if (waypoint == null)
            return;
        controller.Movement.MoveTo(waypoint.transform.position);
    }

    public void Update()
    {
        if (!controller.Movement.HasReachedDestination(10f))
            return;
        controller.ChangeState(controller.WaitingState);
    }

    public void Exit()
    {
    }
}
