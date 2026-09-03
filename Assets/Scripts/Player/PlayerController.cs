using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerController : MonoBehaviour
{
    public float Throttle => throttle;
    public Vector3 Velocity => rb.linearVelocity;

    [SerializeField] private Transform visualModel;
    [SerializeField] private PlayerConfig config;
    [SerializeField] private float windInfluence = 1f;
    [SerializeField] private float hoverDrag = 8f;

    private float throttle;
    private float roll;
    private float pitch;
    private float throttleMultiplier = 1f;
    private float currentFlySpeed = 1f;
    private Vector3 windDirection;
    private float windTimer;
    private float windSpeed;
    private float windLerpSpeed;
    private bool isHovering;
    private WindType currentWindType = WindType.None;

    private Rigidbody rb;
    private PlayerHealth health;
    private Animator animator;
    private PlayerInputController input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();
        input = GetComponent<PlayerInputController>();
        MissionManager.Instance.OnMissionSelected += HandleMissionSelected;
    }

    private void Start()
    {
        animator = visualModel.GetComponent<Animator>();
        animator.SetBool("IsFlying", true);
        health.OnDeath += HandleDeath;
    }

    private void HandleMissionSelected(MissionData mission)
    {
        if (mission is DeliveryMission deliveryMission)
            throttleMultiplier = deliveryMission.throttleMultiplier;
        else if (mission is ArabianNewsMission arabianNewsMission)
            throttleMultiplier = arabianNewsMission.throttleMultiplier;
        else throttleMultiplier = 1f;
    }

    private void HandleInputs()
    {
        if (currentWindType == WindType.Carry)
            return;

        roll = input.Roll;
        pitch = input.Pitch;

        isHovering = input.HoverHeld;
        if (input.ThrottleIncreaseHeld)
            throttle += config.throttleIncrement;
        else if (input.ThrottleDecreaseHeld)
            throttle -= config.throttleIncrement;

        throttle = Mathf.Clamp(throttle, 0f, config.maxThrottle * throttleMultiplier);
    }

    private void Update()
    {
        HandleInputs();
        UpdateFlyAnimationSpeed();
    }

    private void FixedUpdate()
    {
        switch (currentWindType)
        {
            case WindType.Carry:
                HandleWindCarry();
                return;

            case WindType.Assist:
                HandleWindAssist();
                break;
        }

        if (isHovering)
        {
            HandleHover();
            return;
        }

        rb.AddForce(config.maxThrust * throttle * transform.forward);
        rb.AddForce(config.lift * rb.linearVelocity.magnitude * transform.up);

        float turnAmount = roll * config.turnSpeed * Time.fixedDeltaTime;
        Quaternion yawRotation = Quaternion.Euler(0f, turnAmount, 0f);

        float currentPitch = transform.eulerAngles.x;
        if (currentPitch > 180f)
            currentPitch -= 360f;
        float pitchAmount = -pitch * config.pitchSpeed * Time.fixedDeltaTime;
        float targetPitch = Mathf.Clamp(currentPitch + pitchAmount, -config.maxPitchAngle, config.maxPitchAngle);
        Quaternion pitchRotation = Quaternion.Euler(targetPitch, transform.eulerAngles.y, 0f);
        rb.MoveRotation(yawRotation * pitchRotation);

        float visualRoll = -roll * 30f;
        Quaternion targetVisualRotation = Quaternion.Euler(0f, 0f, visualRoll);
        visualModel.localRotation = Quaternion.Lerp(visualModel.localRotation, targetVisualRotation, Time.fixedDeltaTime * 5f);

        rb.angularVelocity = Vector3.zero;
        Vector3 desiredVelocity = transform.forward * rb.linearVelocity.magnitude;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, Time.fixedDeltaTime * 3f);

        ApplyWind();
    }

    private void HandleWindCarry()
    {
        windTimer -= Time.fixedDeltaTime;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, windDirection * windSpeed, Time.fixedDeltaTime * windLerpSpeed);
        if (windTimer <= 0f)
            currentWindType = WindType.None;
    }

    private void HandleWindAssist()
    {
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, windDirection * windSpeed, Time.fixedDeltaTime * windLerpSpeed);
        windTimer -= Time.fixedDeltaTime;
        if (windTimer <= 0f)
            currentWindType = WindType.None;
    }

    private void ApplyWind()
    {
        Vector3 wind = WindManager.Instance.GetWindAtPosition(transform.position);
        rb.linearVelocity += Time.fixedDeltaTime * windInfluence * wind;
    }

    public void StartWindCarry(Vector3 direction, float duration, float speed, float lerpSpeed)
    {
        throttle = 0f;
        currentWindType = WindType.Carry;
        StartWind(direction, duration, speed, lerpSpeed);
    }

    public void StartWindAssist(Vector3 direction, float duration, float speed, float lerpSpeed)
    {
        currentWindType = WindType.Assist;
        StartWind(direction, duration, speed, lerpSpeed);
    }

    private void StartWind(Vector3 direction, float duration, float speed, float lerpSpeed)
    {
        windDirection = direction.normalized;
        windTimer = duration;
        windSpeed = speed;
        windLerpSpeed = lerpSpeed;
    }

    private void HandleHover()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, hoverDrag * Time.fixedDeltaTime);
        float turnAmount = roll * config.turnSpeed * Time.fixedDeltaTime;
        Quaternion yawRotation = Quaternion.Euler(0f, turnAmount, 0f);
        float currentPitch = transform.eulerAngles.x;
        if (currentPitch > 180f)
            currentPitch -= 360f;
        float pitchAmount = -pitch * config.pitchSpeed * Time.fixedDeltaTime;
        float targetPitch = Mathf.Clamp(currentPitch + pitchAmount, -config.maxPitchAngle, config.maxPitchAngle);
        Quaternion pitchRotation = Quaternion.Euler(targetPitch, transform.eulerAngles.y, 0f);
        rb.MoveRotation(yawRotation * pitchRotation);
    }

    private void UpdateFlyAnimationSpeed()
    {
        if (Throttle <= 0.05f)
        {
            animator.SetBool("IsFlying", false);
            animator.SetBool("IsGliding", true);
            return;
        }
        else if (animator.GetBool("IsGliding") == true)
        {
            animator.SetBool("IsGliding", false);
            animator.SetBool("IsFlying", true);
        }

        float targetFlySpeed;
        if (Throttle > 100f)
            targetFlySpeed = 1f;
        else if (Throttle > 75f)
            targetFlySpeed = 0.8f;
        else
            targetFlySpeed = 0.6f;
        if (Mathf.Approximately(currentFlySpeed, targetFlySpeed))
            return;

        currentFlySpeed = targetFlySpeed;
        animator.SetFloat("FlySpeed", currentFlySpeed);
    }

    private void HandleDeath()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 180f);
        animator.SetBool("IsFlying", false);
        animator.SetBool("IsGliding", true);
        throttleMultiplier = 0;
        enabled = false;
    }

    private void OnDestroy()
    {
        MissionManager.Instance.OnMissionSelected -= HandleMissionSelected;
        health.OnDeath -= HandleDeath;
    }

    public void StopWindCarry()
    {
        currentWindType = WindType.None;
    }
}