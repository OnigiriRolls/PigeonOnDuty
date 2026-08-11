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
        TutorialManager.Instance.TryShow("tutorial_enemy_city_parade", "Save the human", "The human is lurred by a parade. Get close and get him back! Guide him to his destination!");
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
            string interact = InputDisplayHelper.Instance.GetHint("Interact");
            human.ShowHint($"Hold {interact} to call back.");
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
