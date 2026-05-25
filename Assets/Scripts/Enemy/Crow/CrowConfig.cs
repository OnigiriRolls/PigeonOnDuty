using UnityEngine;

[CreateAssetMenu(fileName = "CrowConfig", menuName = "Game/Enemies/Crow Config")]
public class CrowConfig : ScriptableObject
{
    [Header("Movement")]
    public float rotationSpeed = 7f;

    [Header("Chase")]
    public float chaseSpeed = 30f;
    public float followTolerance = 20f;
    public float leftOffset = 5f;
    public float minWaitTime = 6f;
    public float maxWaitTime = 8f;

    [Header("Dash")]
    public float dashSpeed = 80f;
    public int maxAttacks = 2;
}
