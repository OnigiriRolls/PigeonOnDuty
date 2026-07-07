using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableData", menuName = "Game/Throwables/Throwable Data")]
public class ThrowableData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ThrowableProjectile projectilePrefab;
    public float minForce = 10f;
    public float maxForce = 35f;
    public float chargeDuration = 1.5f;
    public float gravityMultiplier = 1f;
}
