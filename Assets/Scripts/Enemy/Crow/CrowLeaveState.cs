using UnityEngine;

public class CrowLeaveState : CrowState
{
    private float destroyTimer = 10f;

    public CrowLeaveState(CrowController crowController) : base(crowController)
    {
    }

    public override void UpdateState()
    {
    }
}
