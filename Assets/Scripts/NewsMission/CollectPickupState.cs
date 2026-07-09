using UnityEngine;

public class CollectPickupState : IPedestrianState
{
    private readonly PedestrianController controller;
    private ThrowablePickup pickup;
    private bool collected;

    public CollectPickupState(PedestrianController controller)
    {
        this.controller = controller;
    }

    public void SetPickup(ThrowablePickup pickup)
    {
        this.pickup = pickup;
        collected = false;
    }

    public void Enter()
    {
        controller.Movement.MoveTo(pickup.transform.position);
    }

    public void MoveAndTryCollect()
    {
        if (pickup == null)
        {
            controller.ChangeState(controller.WalkingState);
            return;
        }
        controller.Movement.MoveTo(pickup.transform.position);
        if (controller.Movement.HasReachedDestination(3f))
        {
            controller.PickUp(pickup);
            pickup.Collect();
            collected = true;
            controller.ChangeState(controller.WalkingState);
        }
    }

    public void Exit()
    {
        if (!collected && pickup != null)
            pickup.ReleaseReservation();
    }

    public void Update()
    {
        MoveAndTryCollect();
    }
}
