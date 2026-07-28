using System;
using UnityEngine;

public class DogFetchState : DogState
{
    private ThrowablePickup targetPickup;
    private readonly DogController controller;

    private bool pickedUp;

    public DogFetchState(DogController dog) : base(dog)
    {
        controller = dog;
    }

    public override void Enter()
    {
        pickedUp = false;
        dog.ShowInterest();
    }

    public override void UpdateState()
    {
        if (targetPickup == null && !pickedUp)
        {
            dog.ChangeState(dog.PatrolState);
            return;
        }
        dog.MoveTowards(targetPickup.transform.position, dog.Config.fetchSpeed);
        if (Vector3.Distance(controller.transform.position, targetPickup.transform.position) < 3f)
        {
            pickedUp = controller.PickUp(targetPickup);
            dog.ChangeState(dog.CarryState);
            return;
        }
    }

    public override void Exit()
    {
        dog.HideCarryVisual();
    }

    public void SetTarget(ThrowablePickup pickup)
    {
        targetPickup = pickup;
    }
}
