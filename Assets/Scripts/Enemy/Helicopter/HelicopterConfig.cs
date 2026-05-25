using UnityEngine;

[CreateAssetMenu(fileName = "HelicopterConfig", menuName = "Game/Enemies/Helicopter Config")]
public class HelicopterConfig : ScriptableObject
{
    [Header("Controller")]
    public float moveSpeed = 80f;
    public float forwardDistance = 80f;
    public float upDistance = 10f;
    public float sideOffset = 15f;
    public float homingStrength = 10f;
    public float homingDuration = 0.4f;
    public float shootingDistance = 5f;

    [Header("Gun")]
    public float predictionPrecision = 0.1f;
    public float fireRate = 0.1f;
    public int bulletsToShoot = 40;

    [Header("Bullet")]
    public float bulletSpeed = 300f;
    public float lifetime = 5f;
}
