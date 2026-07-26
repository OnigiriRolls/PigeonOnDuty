using UnityEngine;

public interface IProjectileTarget
{
    bool CanBeHitBy(ThrowableData item);
    bool OnHit(ThrowableData item);
}
