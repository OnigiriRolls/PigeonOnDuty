using UnityEngine;

[CreateAssetMenu(fileName = "WindGustConfig", menuName = "Game/Enemies/Wind Gust Config")]
public class WindGustConfig : ScriptableObject
{
    public float moveSpeed = 15f;
    public float lifeTime = 8f;
    public float carryDuration = 2.5f;
    public float upwardInfluence = 0.5f;
    public float trackingDistance = 8f;

    public float slowThreshold = 75f;
    public float mediumThreshold = 100f;

    public float slowMoveSpeed = 15f;
    public float mediumMoveSpeed = 20f;
    public float fastMoveSpeed = 25f;

    public float GetMoveSpeed(float playerSpeedKmh)
    {
        if (playerSpeedKmh <= slowThreshold)
            return slowMoveSpeed;
        if (playerSpeedKmh <= mediumThreshold)
            return mediumMoveSpeed;
        return fastMoveSpeed;
    }
}
