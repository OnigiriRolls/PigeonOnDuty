using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ParadeMember : MonoBehaviour, INPCMovement
{
    public float CurrentSpeed => agent.velocity.magnitude;
    private NavMeshAgent agent;
    private Vector3 localOffset;
    private ParadeController parade;
    private Vector3 lastTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(ParadeController parade, Vector3 localOffset)
    {
        this.parade = parade;
        this.localOffset = localOffset;
    }

    private void Update()
    {
        if (parade == null)
            return;
        Vector3 target = parade.GetFormationPosition(localOffset);
        if ((target - lastTarget).sqrMagnitude > 1f)
        {
            lastTarget = target;
            agent.SetDestination(target);
        }
    }
}
