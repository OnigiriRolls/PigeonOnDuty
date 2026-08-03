using UnityEngine;

public class GPSDogState
{
    protected readonly GPSDogController dog;

    protected GPSDogState(GPSDogController dog)
    {
        this.dog = dog;
    }

    public virtual void Enter() { }
    public virtual void UpdateState() { }
    public virtual void Exit() { }
}
