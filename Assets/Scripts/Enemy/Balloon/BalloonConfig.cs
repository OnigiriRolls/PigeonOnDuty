using UnityEngine;

[CreateAssetMenu(fileName = "BalloonConfig", menuName = "Game/Enemies/Balloon Config")]
public class BalloonConfig : ScriptableObject
{
    public float steeringStrengthToPlayer = 10f;
    public float steeringStrengthToPlayerDrift = 6f;
    public float driftOffset = 1f;
    public float forwardOffset = 10f;
    public float orbitRadius = 3f;
    public float orbitSpeed = 1f;
    public float minSafeDistance = 1.5f;
    public float moveSpeed = 50f;
    public float minDriftDuration = 10f;
    public float maxDriftDuration = 15f;
}
