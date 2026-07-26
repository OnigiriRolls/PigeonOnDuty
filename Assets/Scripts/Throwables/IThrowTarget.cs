using UnityEngine;

public interface IThrowTarget
{
    Transform AimPoint { get; }

    bool OnHit(ThrowableData item);
    void ShowTargetRing(bool show);
}
