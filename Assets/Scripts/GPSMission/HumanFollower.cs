using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NPCDialogueUI))]
[RequireComponent(typeof(NavMeshAgent))]
public class HumanFollower : MonoBehaviour, INPCMovement
{
    public float DistractionPercent => distractionMeter / maxDistraction;
    public float RecallDuration => recallDuration;
    public float CurrentSpeed { get; private set; }
    public float StateProgress { get; set; }
    public string StateLabel { get; set; }
    public event Action OnTrustDepleted;
    public bool InteractHeld => playerInputController != null && playerInputController.InteractHeld;

    [SerializeField] private LayerMask paradeZoneLayer;
    [SerializeField] private float maxDistraction = 100f;
    [SerializeField] private float recallDuration = 2f;
    [SerializeField] private float paradeSpeedMultiplier = 0.5f;
    [SerializeField] private GameObject recallCircle;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private HumanInteractionZone interactionZone;
    [SerializeField] private float maxPlayerDistance = 80f;
    [SerializeField] private float abandonDelay = 5f;
    [SerializeField] private int maxTrust = 3;
    [SerializeField] private NPCTrustUI trustUI;
    [SerializeField] private AudioClip loseNPCClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject distractionBar;

    private bool IsWaiting => currentState is FollowerWaitingState;
    private bool CanWait => currentState is FollowingState || currentState is FollowerWaitingState;
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
    private float abandonTimer;
    private ParadeController defaultParade;
    private bool waitingPlayer;
    private int trust;
    private HintUI hintUI;
    private PlayerInputController playerInputController;

    private void Awake()
    {
        dialogueUI = GetComponent<NPCDialogueUI>();
        recallCircleRenderer = recallCircle.GetComponent<SpriteRenderer>();
        distractionMeter = maxDistraction;
        agent = GetComponent<NavMeshAgent>();
        ChangeState(new FollowingState(this));
        initialSpeed = agent.speed;
        isInParade = false;
        waitingPlayer = true;
        trust = maxTrust;
        trustUI.Refresh(trust);
    }

    public void Initialize(Transform targetToFollow, ParadeController defaultParade, HintUI hintUI, PlayerInputController playerInputController)
    {
        this.playerInputController = playerInputController;
        if (playerInputController != null)
        {
            playerInputController.OnToggleNPCFollow += ToggleFollowing;
            playerInputController.OnInteract += HandleInteract;
        }
        target = targetToFollow;
        this.defaultParade = defaultParade;
        waitingPlayer = false;
        this.hintUI = hintUI;
    }

    private void Update()
    {
        currentState?.Update();
        UpdateAbandon();
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

    private void UpdateAbandon()
    {
        if (waitingPlayer)
            return;
        if (currentState is not FollowingState)
        {
            ResetAbandon();
            return;
        }
        if (target == null)
        {
            ResetAbandon();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= maxPlayerDistance)
        {
            ResetAbandon();
            return;
        }

        abandonTimer += Time.deltaTime;
        float remaining = Mathf.Ceil(abandonDelay - abandonTimer);
        WarningManager.Instance.Show("Come back!", $"{remaining}");
        if (abandonTimer >= abandonDelay)
        {
            JoinRandomParade();
        }
    }

    private void JoinRandomParade()
    {
        if (currentState is FollowingParadeState)
            return;
        if (defaultParade == null)
        {
            Debug.LogWarning("Human cannot join parade: no valid parade assigned.");
            ResetAbandon();
            return;
        }

        ResetAbandon();
        currentParade = defaultParade;
        isInParade = true;
        ShowMessage("I guess I'll follow them...");
        if (hintUI != null)
            hintUI.Show("The Human got lost and joined a parade...");
        ChangeState(new FollowingParadeState(this));
    }

    public void PlaySound()
    {
        AudioManager.Instance.PlayRandomSFX(loseNPCClip, audioSource);
    }

    public void NotifyParadeNearby(ParadeController parade)
    {
        if (parade == null)
            return;
        currentParade = parade;
        isInParade = true;
        ResetAbandon();
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

    public void ActivateDistractionBar(bool active)
    {
        distractionBar.SetActive(active);
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
            Debug.Log("current parade = null");
            StopAgent();
            return;
        }
        if (!agent.enabled || !agent.isOnNavMesh)
        {
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
        if (currentState is FollowingState || currentState is FollowerWaitingState)
        {
            ResetAbandon();
            ChangeState(new DogCaughtState(this));
        }
    }

    internal void OnDogGone()
    {
        if (currentState is DogCaughtState)
        {
            ResetAbandon();
            ChangeState(new FollowingState(this));
        }
    }

    public void Wait()
    {
        ResetAbandon();
        ChangeState(new FollowerWaitingState(this));
    }

    public void ResumeFollowing()
    {
        ResetAbandon();
        ChangeState(new FollowingState(this));
    }

    public void LoseTrust()
    {
        trust--;
        trust = Mathf.Max(0, trust);
        trustUI.Refresh(trust);
        if (trust == 0)
        {
            OnTrustDepleted?.Invoke();
        }
    }

    private void ResetAbandon()
    {
        abandonTimer = 0f;
        if (WarningManager.Instance != null)
            WarningManager.Instance.Hide();
    }

    private void ToggleFollowing()
    {
        if (!CanWait)
            return;
        if (IsWaiting)
            ResumeFollowing();
        else
            Wait();
    }

    private void HandleInteract()
    {
        if (currentState is FollowingParadeState && CanInteract())
        {
            ChangeState(new BeingRecalledState(this));
        }
    }

    private void OnDisable()
    {
        if (playerInputController != null)
        {
            playerInputController.OnToggleNPCFollow -= ToggleFollowing;
            playerInputController.OnInteract -= HandleInteract;
        }
    }

}
