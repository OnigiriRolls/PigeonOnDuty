public abstract class CrowState
{
    protected BaseCrowController crow;

    public CrowState(BaseCrowController crow)
    {
        this.crow = crow;
    }

    public virtual void Enter() { }
    public virtual void UpdateState() { }
    public virtual void Exit() { }
}
