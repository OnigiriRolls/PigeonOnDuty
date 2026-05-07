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

    [SerializeField] TextMeshProUGUI hud;
    [SerializeField] private Transform visualModel;

    private float throttle;
    private float roll;
    private float pitch;

    Rigidbody rb;

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
        float targetPitch = Mathf.Clamp(
            currentPitch + pitchAmount,
            -maxPitchAngle,
            maxPitchAngle
        );
        Quaternion pitchRotation = Quaternion.Euler(targetPitch, transform.eulerAngles.y, 0f);
        rb.MoveRotation(yawRotation * pitchRotation);

        float visualRoll = -roll * 30f;
        Quaternion targetVisualRotation = Quaternion.Euler(0f, 0f, visualRoll);
        visualModel.localRotation = Quaternion.Lerp(
            visualModel.localRotation,
            targetVisualRotation,
            Time.fixedDeltaTime * 5f
        );

        rb.angularVelocity = Vector3.zero;
    }

    private void UpdateHud()
    {
        hud.text = $"Throttle: {throttle:F0} %{Environment.NewLine}" +
            $"Airspeed: {(rb.linearVelocity.magnitude * 3.6f).ToString("F0")} km/h{Environment.NewLine}" +
            $"Altitude: {transform.position.y:F0} m";
    }
}
