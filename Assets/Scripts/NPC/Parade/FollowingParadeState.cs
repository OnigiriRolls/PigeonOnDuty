using UnityEngine;

public class FollowingParadeState : IGPSHumanState
{
    private readonly HumanFollower human;

    public FollowingParadeState(HumanFollower human)
    {
        this.human = human;
    }

    public void Enter()
    {
        human.ShowMessage("This looks fun!");
        human.ShowInteractionCircle(Color.red);
        human.ActivateDistractionBar(true);
        human.LoseTrust();
        human.PlaySound();
    }

    public void Exit()
    {
        human.HideHint();
    }

    public void Update()
    {
        human.FollowParade();
        if (human.CanInteract())
        {
            human.ShowHint("Hold [E] to call back.");

            if (Input.GetKeyDown(KeyCode.E))
            {
                human.ChangeState(new BeingRecalledState(human));
            }
        }
        else
        {
            human.HideHint();
        }
    }

    public void FixedUpdate()
    {
    }
}
