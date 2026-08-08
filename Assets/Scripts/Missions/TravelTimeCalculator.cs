using UnityEngine;

public class TravelTimeCalculator : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float expectedSpeedKmh = 80f;

    public float CalculateTime(Transform target, float timeBuffer, float speed = 0)
    {
        float actualSpeed = speed == 0 ? expectedSpeedKmh : speed;
        float distance = Vector3.Distance(player.position, target.position);
        float expectedSpeedMs = actualSpeed / 3.6f;
        return distance / expectedSpeedMs + timeBuffer;
    }
}
