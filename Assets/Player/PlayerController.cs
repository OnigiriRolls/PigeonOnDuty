using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hud;
    public float throttleIncrement = 0.1f;
    public float maxThrust = 200f;
    public float responsiveness = 10f;
    public float lift = 135f;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;

    Rigidbody rb;

    private float responseModifier
    {
        get
        {
            return (rb.mass / 10f) * responsiveness;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HandleInputs()
    {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

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
        // throttle = percentage
        rb.AddForce(maxThrust * throttle * transform.forward);
        // rotation
        rb.AddTorque(responseModifier * yaw * transform.up);
        rb.AddTorque(pitch * responseModifier * transform.right);
        rb.AddTorque(responseModifier * roll * -transform.forward);
        rb.AddForce(lift * rb.linearVelocity.magnitude * Vector3.up);
    }

    private void UpdateHud()
    {
        hud.text = $"Throttle: {throttle:F0} %{Environment.NewLine}" +
            $"Airspeed: {(rb.linearVelocity.magnitude * 3.6f).ToString("F0")} km/h{Environment.NewLine}" +
            $"Altitude: {transform.position.y:F0} m";
    }
}
