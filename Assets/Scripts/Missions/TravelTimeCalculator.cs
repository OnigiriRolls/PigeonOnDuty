using UnityEngine;

public class TravelTimeCalculator : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float expectedSpeedKmh = 80f;

    public float CalculateTime(Transform target, float timeBuffer)
    {
        float distance = Vector3.Distance(player.position, target.position);
        float expectedSpeedMs = expectedSpeedKmh / 3.6f;
        return distance / expectedSpeedMs + timeBuffer;
    }
}
