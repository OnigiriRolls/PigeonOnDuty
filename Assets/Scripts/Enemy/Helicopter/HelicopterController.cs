using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private HelicopterGun[] guns;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private int bulletsToShoot = 20;

    [SerializeField] private Transform player;
    private Vector3 spawnPosition;
    [SerializeField] private HelicopterManager manager;
    private int bulletsShot;
    private float fireTimer;
    private float sideOffset;
    private HelicopterState currentState;
    private Transform visualModel;

    private enum HelicopterState
    {
        Entering,
        Shooting,
        Leaving
    }

    public void Initialize(Transform targetPlayer, Vector3 originalSpawn, HelicopterManager helicopterManager)
    {
        visualModel = transform.GetChild(0);
        player = targetPlayer;
        spawnPosition = originalSpawn;
        manager = helicopterManager;
        sideOffset = Random.Range(-15f, 15f);
        currentState = HelicopterState.Entering;
    }

    private void Update()
    {
        switch (currentState)
        {
            case HelicopterState.Entering:
                HandleEntering();
                //HandleShooting();
                break;

            case HelicopterState.Shooting:
                HandleShooting();
                break;

            case HelicopterState.Leaving:
                HandleLeaving();
                break;
        }
    }

    private void HandleEntering()
    {
        Vector3 attackPosition = GetAttackPosition();
        MoveTowards(attackPosition);
        float distance = Vector3.Distance(transform.position, attackPosition);
        if (distance < 5f)
        {
            currentState = HelicopterState.Shooting;
        }
    }

    private void HandleShooting()
    {
        transform.LookAt(player);
        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f)
            return;

        fireTimer = fireRate;
        Shoot();
        bulletsShot++;
        if (bulletsShot >= bulletsToShoot)
        {
            currentState = HelicopterState.Leaving;
        }
    }

    private void HandleLeaving()
    {
        MoveTowards(spawnPosition);
        float distance = Vector3.Distance(transform.position, spawnPosition);
        if (distance < 10f)
        {
            manager.HelicopterFinished();
            Destroy(gameObject);
        }
    }

    private void MoveTowards(Vector3 target)
    {
        //return;
        Vector3 dir = (target - transform.position).normalized;
        transform.position += moveSpeed * Time.deltaTime * dir;

        //float tiltZ = Vector3.Dot(dir, transform.right) * -10f;
        //float tiltX = Vector3.Dot(dir, transform.forward) * 5f;
        //Quaternion visualRotation = Quaternion.Euler(tiltX, 0f, tiltZ);
        //visualModel.localRotation = Quaternion.Lerp(visualModel.localRotation, visualRotation, Time.deltaTime * 3f);
    }

    private Vector3 GetAttackPosition()
    {
        return player.position + player.forward * manager.ForwardDistance + player.right * sideOffset + Vector3.up * manager.UpDistance;
    }

    private void Shoot()
    {
        foreach (HelicopterGun gun in guns)
        {
            gun.Shoot(player, manager);
        }
    }
}
