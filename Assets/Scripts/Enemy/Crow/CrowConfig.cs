using UnityEngine;

[CreateAssetMenu(fileName = "CrowConfig", menuName = "Game/Enemies/Crow Config")]
public class CrowConfig : ScriptableObject
{
    [Header("General")]
    public CrowAbility ability;
    
    [Header("Movement")]
    public float rotationSpeed = 7f;

    [Header("Patrol")]
    public float patrolSpeed = 10f;

    [Header("Chase")]
    public float chaseSpeed = 30f;
    public float followTolerance = 20f;
    public float leftOffset = 5f;
    public float minWaitTime = 6f;
    public float maxWaitTime = 8f;

    [Header("Dash")]
    public float dashSpeed = 80f;
    public int maxAttacks = 2;

    [Header("Return")]
    public float safeReturnAltitude = 70f;
    public float returnTolerance = 5f;

    [Header("Detection")]
    public float detectionRadius = 120f;
}
