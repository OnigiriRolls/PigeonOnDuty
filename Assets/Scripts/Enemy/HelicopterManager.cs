using UnityEngine;
using UnityEngine.Rendering;

public class HelicopterManager : MonoBehaviour
{
    public float ForwardDistance => forwardDistance;
    public float UpDistance => upDistance;
    public float PredictionPrecision => predictionPrecision;
    public float BulletSpeed => bulletSpeed;
    public float HomingStrength => homingStrength;
    public float HomingDuration => homingDuration;

    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject helicopterPrefab;
    [SerializeField] private float highAltitude = 151f;
    [SerializeField] private float spawnChance = 0.3f;
    [SerializeField] private float spawnCheckInterval = 5f;
    [SerializeField] private float forwardDistance = 50f;
    [SerializeField] private float upDistance = 50f;
    [SerializeField] private float predictionPrecision = 0.5f;
    [SerializeField] private float bulletSpeed = 150f;
    [SerializeField] private float homingStrength = 2f;
    [SerializeField] private float homingDuration = 0.4f;


    private bool helicopterActive;
    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;
        timer = spawnCheckInterval;
        if (helicopterActive)
            return;
        if (player.transform.position.y < highAltitude)
            return;
        if (Random.value > spawnChance)
            return;

        SpawnHelicopter();
    }

    private void SpawnHelicopter()
    {
        Vector3 spawnPosition = player.transform.GetChild(0).position;
        GameObject helicopter = Instantiate(helicopterPrefab, spawnPosition, Quaternion.identity);
        HelicopterControllerSimple controller = helicopter.GetComponent<HelicopterControllerSimple>();
        controller.Initialize(player.transform, spawnPosition, this);
        helicopterActive = true;
    }

    public void HelicopterFinished()
    {
        helicopterActive = false;
    }
}
