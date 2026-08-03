using System;
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
    private bool isInParade;
    private ParadeController currentParade;

    private void Awake()
    {
        dialogueUI = GetComponent<NPCDialogueUI>();
        recallCircleRenderer = recallCircle.GetComponent<SpriteRenderer>();
        distractionMeter = maxDistraction;
        agent = GetComponent<NavMeshAgent>();
        ChangeState(new FollowingState(this));
        initialSpeed = agent.speed;
        isInParade = false;
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
        currentState?.FixedUpdate();
    }

    public void ChangeState(IGPSHumanState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void NotifyParadeNearby(ParadeController parade)
    {
        isInParade = true;
        currentParade = parade;
    }

    public bool IsInParade()
    {
        return isInParade;
    }

    public bool IsNearParade()
    {
        if (currentParade == null)
            return false;

        return Vector3.Distance(transform.position, currentParade.transform.position) < 105f;
    }

    public void RefreshDistractionMeter()
    {
        distractionMeter = maxDistraction;
        isInParade = false;
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
        if (isInParade && !speedApplied)
        {
            agent.speed *= paradeSpeedMultiplier;
            speedApplied = true;
        }
        if (!isInParade && speedApplied)
        {
            speedApplied = false;
            agent.speed = initialSpeed;
        }
        CurrentSpeed = agent.velocity.magnitude;
    }

    public void FollowParade()
    {
        if (currentParade == null)
        {
            StopAgent();
            return;
        }
        agent.isStopped = false;
        Vector3 targetPosition = currentParade.transform.position;
        targetPosition.y = transform.position.y;
        if (Vector3.Distance(transform.position, targetPosition) > 4f)
        {
            agent.SetDestination(targetPosition);
        }
        if (isInParade && !speedApplied)
        {
            agent.speed *= paradeSpeedMultiplier;
            speedApplied = true;
        }
        if (!isInParade && speedApplied)
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

    internal void OnDogScared()
    {
        if (currentState is DogCaughtState)
            return;
        ChangeState(new DogCaughtState(this));
    }

    internal void OnDogGone()
    {
        if (currentState is DogCaughtState)
            ChangeState(new FollowingState(this));
    }
}
