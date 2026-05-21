using UnityEngine;

public class BalloonManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject balloonPrefab;

    [Header("Spawn")]
    [SerializeField] private float spawnCheckInterval = 5f;
    [SerializeField] private float spawnChance = 0.4f;
    [SerializeField] private float midAltitude = 70f;
    [SerializeField] private Transform spawnPosition;

    [Header("Drift")]
    public float steeringStrengthToPlayer = 10f;
    public float steeringStrengthToPlayerDrift = 10f;
    public float driftStrength = 1f;
    public float driftFrequency = 1f;
    public float driftOffset = 1f;
    public float forwardOffset = 1f;
    public float orbitRadius = 12f;
    public float orbitSpeed = 1f;
    public float minSafeDistance = 1.5f;
    public float moveSpeed = 20f;
    public float minDriftDuration = 10f;
    public float maxDriftDuration = 15f;

    private bool balloonActive;
    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;
        timer = spawnCheckInterval;
        TrySpawnBalloon();
    }

    private void TrySpawnBalloon()
    {
        if (balloonActive)
            return;
        if (player.transform.position.y < midAltitude)
            return;
        if (Random.value > spawnChance)
            return;
        SpawnBalloon();
    }

    private void SpawnBalloon()
    {
        GameObject balloon = Instantiate(balloonPrefab, spawnPosition.position, Quaternion.identity);
        BalloonController controller = balloon.GetComponent<BalloonController>();
        controller.Initialize(player.transform, this);
        balloonActive = true;
    }

    public void BalloonFinished()
    {
        balloonActive = false;
    }
}
