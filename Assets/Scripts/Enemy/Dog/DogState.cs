using UnityEngine;

public abstract class DogState
{
    protected readonly DogController dog;

    protected DogState(DogController dog)
    {
        this.dog = dog;
    }

    public virtual void Enter() { }
    public virtual void UpdateState() { }
    public virtual void Exit() { }
}
