using UnityEngine;

public class FlightController : MonoBehaviour
{
    [Header("Speed")]
    public float forwardSpeed = 10f;
    public float strafeSpeed = 8f;
    public float verticalSpeed = 6f;

    [Header("Smoothing")]
    public float acceleration = 5f;
    public float deceleration = 6f;

    private Vector3 currentVelocity;
    private PlayerInputHandler input;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 targetVelocity = new Vector3(
            input.MoveInput.x * strafeSpeed,
            input.VerticalInput * verticalSpeed,
            input.MoveInput.y * forwardSpeed
        );


        float accelRate = (targetVelocity.magnitude > 0.1f) ? acceleration : deceleration;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, accelRate * Time.deltaTime);
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);
    }
}
