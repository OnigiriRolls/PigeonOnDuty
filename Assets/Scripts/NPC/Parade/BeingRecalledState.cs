using UnityEngine;

public class BeingRecalledState : IGPSHumanState
{
    private readonly HumanFollower human;
    private float recallProgress;

    public BeingRecalledState(HumanFollower human)
    {
        this.human = human;
    }

    public void Enter()
    {
        recallProgress = 0f;
        human.ShowMessage("Wait, I'm coming!");
        human.ShowHint("Hold [E]...");
      //  human.StateLabel = "CALLING BACK";
    }

    public void Exit()
    {
        human.HideHint();
    }

    public void Update()
    {
        human.FollowParade();
        if (!human.CanInteract())
        {
            //human.ChangeState(new FollowingParadeState(human));
            human.ShowHint("Get closer!");
            return;
        }
        human.ShowHint("Hold [E]...");
        if (!Input.GetKey(KeyCode.E))
        {
            //recallProgress = 0f;
            //human.StateProgress = 0f;
            return;
        }
        recallProgress += Time.deltaTime;
        human.StateProgress = recallProgress / human.RecallDuration;
        if (recallProgress >= human.RecallDuration)
        {
            human.StateLabel = "";
            human.HideInteractionCircle();
            human.ShowMessage("I lost you for a second...");
            human.ChangeState(new ReturningState(human));
        }
    }

    public void FixedUpdate()
    {
    }
}
