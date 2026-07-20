using UnityEngine;

public interface IThrowTarget
{
    Transform AimPoint { get; }

    void OnHit(ThrowableData item);
    void ShowTargetRing(bool show);
}
