using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float throttleIncrement = 0.7f;
    public float maxThrust = 200f;
    public float responsiveness = 10f;
    public float lift = 135f;
    public float turnSpeed = 90f;
    public float pitchSpeed = 30f;
    public float maxPitchAngle = 40f;
    public float Throttle => throttle;
    public float Roll => roll;
    public float Pitch => pitch;
    public bool IsFlying => isFlying;
    public bool IsGliding => isGliding;
    public bool IsGrounded => isGrounded;
    public Vector3 Velocity => rb.linearVelocity;

    [SerializeField] private TextMeshProUGUI hud;
    [SerializeField] private Transform visualModel;
    [SerializeField] private LayerMask landingAreaLayer;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private LayerMask obstacleLayer;

    private float throttle;
    private float roll;
    private float pitch;
    private bool isGrounded;
    private bool isFlying;
    private bool isGliding;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HandleInputs()
    {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");

        if (Input.GetKey(KeyCode.Space)) throttle += throttleIncrement;
        else if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement;

        throttle = Mathf.Clamp(throttle, 0f, 100f);
    }

    private void Update()
    {
        HandleInputs();
        UpdateHud();
        CheckGrounded();
        UpdateFlightState();
        UpdateGlidingState();
    }

    private void FixedUpdate()
    {
        rb.AddForce(maxThrust * throttle * transform.forward);
        rb.AddForce(lift * rb.linearVelocity.magnitude * transform.up);

        float turnAmount = roll * turnSpeed * Time.fixedDeltaTime;
        Quaternion yawRotation = Quaternion.Euler(0f, turnAmount, 0f);

        float currentPitch = transform.eulerAngles.x;
        if (currentPitch > 180f)
            currentPitch -= 360f;
        float pitchAmount = -pitch * pitchSpeed * Time.fixedDeltaTime;
        float targetPitch = Mathf.Clamp(currentPitch + pitchAmount, -maxPitchAngle, maxPitchAngle);
        Quaternion pitchRotation = Quaternion.Euler(targetPitch, transform.eulerAngles.y, 0f);
        rb.MoveRotation(yawRotation * pitchRotation);

        float visualRoll = -roll * 30f;
        Quaternion targetVisualRotation = Quaternion.Euler(0f, 0f, visualRoll);
        visualModel.localRotation = Quaternion.Lerp(visualModel.localRotation, targetVisualRotation, Time.fixedDeltaTime * 5f);

        rb.angularVelocity = Vector3.zero;
        Vector3 desiredVelocity = transform.forward * rb.linearVelocity.magnitude;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, Time.fixedDeltaTime * 3f);
    }


    private void UpdateHud()
    {
        hud.text = $"Throttle: {throttle:F0} %{Environment.NewLine}" +
            $"Airspeed: {rb.linearVelocity.magnitude * 3.6f:F0} km/h{Environment.NewLine}" +
            $"Altitude: {transform.position.y:F0} m";
    }

    private void UpdateFlightState()
    {
        if (!isFlying && throttle >= 2f)
        {
            isFlying = true;
        }

        if (isFlying && isGrounded && throttle < 2f)
        {
            isFlying = false;
        }
    }

    private void UpdateGlidingState()
    {
        bool hasMovementInput = Mathf.Abs(roll) > 0.1f || Mathf.Abs(pitch) > 0.1f;
        bool changingThrottle = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl);
        isGliding = isFlying && !hasMovementInput && !changingThrottle;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            landingAreaLayer
        );

        Debug.DrawRay(
             transform.position,
             Vector3.down * groundCheckDistance,
             Color.red
        );
    }
}