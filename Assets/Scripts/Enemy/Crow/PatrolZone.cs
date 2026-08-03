using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SphereCollider))]
public class PatrolZone : MonoBehaviour
{
    public bool HasNewspapers => newspapers.Count > 0;
    public Vector3 Center => transform.position;
    public float Radius => radius;
    public bool IsPlayerInside { get; private set; }
    public HumanFollower CurrentHuman { get; private set; }

    [SerializeField] private float radius = 60f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private int maxAttemptsPerPoint = 10;
    [SerializeField] private float pointCheckRadius = 5f;
    [SerializeField] private int patrolPointCount = 20;

    private readonly List<Vector3> patrolPoints = new();
    private readonly List<NewspaperProjectile> newspapers = new();
    private SphereCollider trigger;

    private void Awake()
    {
        trigger = GetComponent<SphereCollider>();
    }

    public void InitPatrolPoints(float radius)
    {
        this.radius = radius;
        trigger.radius = radius;
        GeneratePatrolPoints();
    }

    public void InitNavMeshPoints(float radius)
    {
        this.radius = radius;
        trigger.radius = radius;
        GenerateNavMeshPoints();
    }

    private void GeneratePatrolPoints()
    {
        patrolPoints.Clear();
        int attempts = 0;
        int maxGenerationAttempts = patrolPointCount * maxAttemptsPerPoint;

        while (patrolPoints.Count < patrolPointCount && attempts < maxGenerationAttempts)
        {
            attempts++;
            Vector2 offset = Random.insideUnitCircle * Radius;
            Vector3 point = Center + new Vector3(offset.x, 0f, offset.y);
            if (Physics.CheckSphere(point, pointCheckRadius, obstacleMask))
                continue;
            patrolPoints.Add(point);
        }

        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("Couldn't generate patrol points.");
            patrolPoints.Add(Center);
        }
    }

    private void GenerateNavMeshPoints()
    {
        patrolPoints.Clear();
        int attempts = 0;
        int maxGenerationAttempts = patrolPointCount * maxAttemptsPerPoint;
        while (patrolPoints.Count < patrolPointCount && attempts < maxGenerationAttempts)
        {
            attempts++;
            Vector2 offset = Random.insideUnitCircle * Radius;
            Vector3 candidate = Center + new Vector3(offset.x, 0f, offset.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                patrolPoints.Add(hit.position);
            }
        }

        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("Couldn't generate nav mesh points.");
            patrolPoints.Add(Center);
        }
    }

    public NewspaperProjectile GetClosestNewspaper(Vector3 position)
    {
        newspapers.RemoveAll(n => n == null);
        NewspaperProjectile closest = null;
        float closestDistance = float.MaxValue;
        foreach (var newspaper in newspapers)
        {
            if (!newspaper.IsPickup)
                continue;
            float sqrDistance = (newspaper.transform.position - position).sqrMagnitude;
            if (sqrDistance < closestDistance)
            {
                closestDistance = sqrDistance;
                closest = newspaper;
            }
        }

        return closest;
    }

    public Vector3 GetRandomPoint()
    {
        return patrolPoints[Random.Range(0, patrolPoints.Count)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            IsPlayerInside = true;
        else if (other.CompareTag("Newspaper"))
        {
            NewspaperProjectile newspaper = other.GetComponent<NewspaperProjectile>();
            newspapers.Add(newspaper);
        }
        else if (other.CompareTag("GPSHuman"))
        {
            CurrentHuman = other.GetComponent<HumanFollower>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            IsPlayerInside = false;
        else if (other.CompareTag("Newspaper"))
        {
            NewspaperProjectile newspaper = other.GetComponent<NewspaperProjectile>();
            newspapers.Remove(newspaper);
        }
        else if (other.CompareTag("GPSHuman"))
        {
            CurrentHuman = null;
        }
    }

    public bool Contains(Vector3 position)
    {
        return (position - transform.position).sqrMagnitude <= radius * radius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Center, Radius);
        Gizmos.color = Color.yellow;
        foreach (Vector3 point in patrolPoints)
        {
            Gizmos.DrawSphere(point, 0.5f);
        }
    }
}
