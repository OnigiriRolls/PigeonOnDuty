using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    public float Throttle => throttle;
    public float Roll => roll;
    public float Pitch => pitch;
    public bool IsFlying => isFlying;
    public bool IsGliding => isGliding;
    public bool IsGrounded => isGrounded;
    public Vector3 Velocity => rb.linearVelocity;

    [SerializeField] private Transform visualModel;
    [SerializeField] private LayerMask landingAreaLayer;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private PlayerConfig config;
    [SerializeField] private DeliveryMissionManager missionManager;

    private float throttle;
    private float roll;
    private float pitch;
    private bool isGrounded;
    private bool isFlying;
    private bool isGliding;
    private float throttleMultiplier = 1f;

    private Rigidbody rb;
    private PlayerHealth health;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        animator = visualModel.GetComponent<Animator>();
        animator.SetBool("IsFlying", true);
        missionManager.OnMissionSelected += HandleMissionSelected;
        health.OnDeath += HandleDeath;
    }

    private void HandleMissionSelected(DeliveryMission mission)
    {
        throttleMultiplier = mission.throttleMultiplier;
    }

    private void HandleInputs()
    {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");

        if (Input.GetKey(KeyCode.Space)) throttle += config.throttleIncrement;
        else if (Input.GetKey(KeyCode.LeftControl)) throttle -= config.throttleIncrement;

        throttle = Mathf.Clamp(throttle, 0f, config.maxThrottle * throttleMultiplier);
    }

    private void Update()
    {
        HandleInputs();
        CheckGrounded();
        UpdateFlightState();
        UpdateGlidingState();
    }

    private void FixedUpdate()
    {
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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, landingAreaLayer);
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
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
        missionManager.OnMissionSelected -= HandleMissionSelected;
        health.OnDeath -= HandleDeath;
    }
}