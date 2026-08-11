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
        string interact = InputDisplayHelper.Instance.GetHint("Interact");
        human.ShowHint($"Hold {interact}...");
      //  human.StateLabel = "CALLING BACK";
    }

    public void Exit()
    {
        human.HideHint();
        human.ActivateDistractionBar(false);
    }

    public void Update()
    {
        human.FollowParade();
        if (!human.InteractHeld)
        {
            human.ShowHint("Get closer!");
            return;
        }
        string interact = InputDisplayHelper.Instance.GetHint("Interact");
        human.ShowHint($"Hold {interact}...");
        if (!human.InteractHeld)
            return;
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
