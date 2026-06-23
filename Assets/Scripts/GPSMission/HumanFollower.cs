using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NPCDialogueUI))]
[RequireComponent(typeof(NavMeshAgent))]
public class HumanFollower : MonoBehaviour, INPCMovement
{
    public float DistractionPercent => distractionMeter / maxDistraction;
    public float DistractionReactionTime => distractionReactionTime;
    public float RecallDuration => recallDuration;
    public float ParadeDuration => paradeDuration;
    public float CurrentSpeed { get; private set; }
    public float StateProgress { get; set; }
    public string StateLabel { get; set; }

    [SerializeField] private LayerMask paradeZoneLayer;
    [SerializeField] private float maxDistraction = 100f;
    [SerializeField] private float distractionDecayRate = 25f;
    [SerializeField] private float distractionRecoveryRate = 10f;
    [SerializeField] private float paradeCheckRadius = 0.5f;
    [SerializeField] private float distractionReactionTime = 5f;
    [SerializeField] private float recallDuration = 2f;
    [SerializeField] private float paradeDuration = 10f;
    [SerializeField] private float paradeSpeedMultiplier = 0.5f;
    [SerializeField] private GameObject recallCircle;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private HumanInteractionZone interactionZone;

    private Transform target;
    private float distractionMeter;
    private NPCDialogueUI dialogueUI;
    private SpriteRenderer recallCircleRenderer;
    private IGPSHumanState currentState;
    private NavMeshAgent agent;
    private bool speedApplied;
    private float initialSpeed;

    private void Awake()
    {
        dialogueUI = GetComponent<NPCDialogueUI>();
        recallCircleRenderer = recallCircle.GetComponent<SpriteRenderer>();
        distractionMeter = maxDistraction;
        agent = GetComponent<NavMeshAgent>();
        ChangeState(new FollowingState(this));
        initialSpeed = agent.speed;
    }

    public void Initialize(Transform targetToFollow)
    {
        target = targetToFollow;
    }

    private void Update()
    {
        currentState?.Update();
    }

    private void FixedUpdate()
    {
        UpdateDistraction();
        currentState?.FixedUpdate();
    }

    public void ChangeState(IGPSHumanState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void UpdateDistraction()
    {
        if (IsInsideParade())
        {
            distractionMeter -= distractionDecayRate * Time.fixedDeltaTime;
            Debug.DrawRay(transform.position, Vector3.forward * 5f, Color.red);
        }
        else
            distractionMeter += distractionRecoveryRate * Time.fixedDeltaTime;
        distractionMeter = Mathf.Clamp(distractionMeter, 0f, maxDistraction);
    }

    private bool IsInsideParade()
    {
        return Physics.CheckSphere(transform.position, paradeCheckRadius, paradeZoneLayer);
    }

    public bool IsDistracted()
    {
        return distractionMeter <= 0f;
    }

    public void RefreshDistractionMeter()
    {
        distractionMeter = maxDistraction;
    }

    public void ShowMessage(string message)
    {
        dialogueUI.ShowMessage(message);
    }

    public void ShowInteractionCircle(Color color)
    {
        recallCircleRenderer.color = color;
        recallCircle.SetActive(true);
    }

    public void HideInteractionCircle()
    {
        recallCircle.SetActive(false);
    }

    public void ShowHint(string message)
    {
        if (hintPanel.activeSelf && hintText.text == message)
            return;
        hintText.text = message;
        hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        hintPanel.SetActive(false);
    }

    public void StopAgent()
    {
        agent.isStopped = true;
        CurrentSpeed = 0f;
    }

    public void FollowTarget()
    {
        if (target == null)
        {
            StopAgent();
            return;
        }
        agent.isStopped = false;
        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;
        agent.SetDestination(targetPosition);
        bool isInsideParade = IsInsideParade();
        if (isInsideParade && !speedApplied)
        {
            agent.speed = agent.speed * paradeSpeedMultiplier;
            speedApplied = true;
        }
        if (!isInsideParade)
        {
            speedApplied = false;
            agent.speed = initialSpeed;
        }
        CurrentSpeed = agent.velocity.magnitude;
    }

    public bool CanInteract()
    {
        return interactionZone.PlayerInside;
    }
}
