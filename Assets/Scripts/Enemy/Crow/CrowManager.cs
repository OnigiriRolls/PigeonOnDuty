using UnityEngine;

public class CrowManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject crowPrefab;

    [Header("Spawn")]
    [SerializeField] private float lowAltitude = 60f;
    [SerializeField] private float spawnCheckInterval = 5f;
    [SerializeField] private float spawnChance = 0.4f;
    [SerializeField] private Transform spawnPosition;

    private bool crowActive;
    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;
        timer = spawnCheckInterval;
        TrySpawnCrow();
    }

    private void TrySpawnCrow()
    {
        if (crowActive)
            return;
        if (player.transform.position.y > lowAltitude)
            return;
        if (Random.value > spawnChance)
            return;
        SpawnCrow();
    }

    private void SpawnCrow()
    {
        GameObject crow = Instantiate(crowPrefab, spawnPosition.position, Quaternion.identity);
        CrowController crowController = crow.GetComponent<CrowController>();
        crowController.Initialize(player.transform, this);
        crowActive = true;
    }

    public void CrowFinished()
    {
        crowActive = false;
    }
}
