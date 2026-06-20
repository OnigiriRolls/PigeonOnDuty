using UnityEngine;

public class DistractedState : IGPSHumanState
{
    private readonly HumanFollower human;
    private float timer;

    public DistractedState(HumanFollower human)
    {
        this.human = human;
    }

    public void Enter()
    {
        Debug.Log("Entering DistractedState State");
        human.StopMoving();
        human.ShowMessage("I'll join them.");
        human.ShowInteractionCircle(Color.red);
        timer = 0f;
        //human.StateLabel = "RECALL NOW";
    }

    public void Exit()
    {
        Debug.Log("Exiting DistractedState State");
    }

    public void Update()
    {
        timer += Time.deltaTime;
        human.StateProgress = 1f - timer / human.DistractionReactionTime;

        if (timer >= human.DistractionReactionTime)
        {
            human.HideHint();
            human.HideInteractionCircle();
            human.ChangeState(new JoinedParadeState(human));
            return;
        }
        if (human.CanRecallNow())
            human.ShowHint("Press [E] to call back.");
        else
        {
            human.HideHint();
            return;
        }
        if (!Input.GetKeyDown(KeyCode.E))
            return;
        human.ChangeState(new BeingRecalledState(human));
    }

    public void FixedUpdate()
    {
    }
}
