using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SphereCollider))]
public class PatrolZone : MonoBehaviour
{
    [SerializeField] private float radius = 60f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private int maxAttemptsPerPoint = 10;
    [SerializeField] private float pointCheckRadius = 5f;
    [SerializeField] private int patrolPointCount = 20;

    private readonly List<Vector3> patrolPoints = new();

    public Vector3 Center => transform.position;
    public float Radius => radius;
    public bool IsPlayerInside { get; private set; }

    private SphereCollider trigger;

    private void Awake()
    {
        trigger = GetComponent<SphereCollider>();
        trigger.radius = radius;
        GeneratePatrolPoints();
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
        trigger.radius = radius;
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

    public Vector3 GetRandomPoint()
    {
        return patrolPoints[Random.Range(0, patrolPoints.Count)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            IsPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            IsPlayerInside = false;
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
