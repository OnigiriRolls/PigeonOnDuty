using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ParadeController : MonoBehaviour, INPCMovement
{
    public float CurrentSpeed => agent.velocity.magnitude;

    [SerializeField] private int nearbyPointCount = 4;
    [SerializeField] private float distractionRadius = 10f;
    [SerializeField] private LayerMask humanLayer;
    [SerializeField] private ParadeMember[] members;
    [SerializeField] private Vector3[] offsets = { new(-2, 0, 2), new(0, 0, 2), new(2, 0, 2), new(-1, 0, 0), new(1, 0, 0), new(-2, 0, -2), new(0, 0, -2), new(2, 0, -2), };
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    private NavMeshAgent agent;
    private Transform player;
    private List<PedestrianSpawnpoint> paradePoints;
    private PedestrianSpawnpoint currentPoint;
    private PedestrianSpawnpoint previousPoint;
    private float followPlayerProbability;
    private bool activated;
    private Vector3 currentDirection = Vector3.forward;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
    }

    public void Initialize(Transform player, List<PedestrianSpawnpoint> paradePoints, PedestrianSpawnpoint startPoint, float followPlayerProbability)
    {
        this.player = player;
        this.paradePoints = paradePoints;
        this.followPlayerProbability = followPlayerProbability;
        currentPoint = startPoint;
        transform.position = startPoint.transform.position;
        agent.Warp(startPoint.transform.position);
        InitializeFormation();
        AudioManager.Instance.PlayRandomSFX(clips, audioSource);
    }

    private void InitializeFormation()
    {
        for (int i = 0; i < members.Length; i++)
        {
            Vector3 offset = i < offsets.Length ? offsets[i] : Random.insideUnitSphere * 3f;
            offset.y = 0;
            members[i].Initialize(this, offset);
        }
    }


    private void Update()
    {
        NotifyNearbyHumans();
        if (!activated)
        {
            activated = true;
            agent.isStopped = false;
            MoveToNextPoint();
            return;
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToNextPoint();
        }
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            currentDirection = agent.velocity.normalized;
        }
    }

    private void MoveToNextPoint()
    {
        PedestrianSpawnpoint next = ChooseNextPoint();
        if (next == null)
            return;
        previousPoint = currentPoint;
        currentPoint = next;
        agent.SetDestination(currentPoint.transform.position);
    }

    private PedestrianSpawnpoint ChooseNextPoint()
    {
        List<PedestrianSpawnpoint> candidates = paradePoints
            .Where(p => p != currentPoint && p != previousPoint)
            .ToList();

        if (candidates.Count == 0)
            return null;

        if (Random.value < followPlayerProbability)
        {
            candidates = candidates
                .OrderBy(p => Vector3.Distance(player.position, p.transform.position))
                .ToList();

            int count = Mathf.Min(nearbyPointCount, candidates.Count);
            return candidates[Random.Range(0, count)];
        }
        return candidates[Random.Range(0, candidates.Count)];
    }

    private void NotifyNearbyHumans()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, distractionRadius, humanLayer);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out HumanFollower human))
            {
                human.NotifyParadeNearby(this);
            }
        }
    }

    public Vector3 GetFormationPosition(Vector3 localOffset)
    {
        Quaternion rotation = Quaternion.LookRotation(currentDirection);
        return transform.position + rotation * localOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distractionRadius);
    }
}
