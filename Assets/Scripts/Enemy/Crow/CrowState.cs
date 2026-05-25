public abstract class CrowState
{
    protected CrowController crow;

    public CrowState(CrowController crow)
    {
        this.crow = crow;
    }

    public virtual void Enter() { }
    public virtual void UpdateState() { }
    public virtual void Exit() { }
}
