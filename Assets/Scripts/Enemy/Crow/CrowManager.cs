using UnityEngine;

public class CrowManager : MonoBehaviour
{
    [SerializeField] private GameObject crowPrefab;
    [SerializeField] private GameObject[] crowPrefabsToInit;
    [SerializeField] private Transform player;
    [SerializeField] private EndlessRunManager endlessRunManager;
    [SerializeField] private float spawnChance = 0.5f;

    private CrowController activeCrow;

    private void Start()
    {
        //foreach (var crow in crowPrefabsToInit)
        //{
        //    crow.GetComponent<CrowController>().Initialize(player, this);
        //}
    }

    public void TrySpawnCrow()
    {
        //// already active crow
        //if (activeCrow != null)
        //    return;

        //// random chance
        //if (Random.value > spawnChance)
        //    return;

        //Vector3 spawnPos = GetSpawnPosition();
        //GameObject crowObj = Instantiate(crowPrefab, spawnPos, Quaternion.identity);
        //activeCrow = crowObj.GetComponent<CrowController>();
        //activeCrow.Initialize(player, this);
    }

    private Vector3 GetSpawnPosition()
    {
        Transform checkpoint = endlessRunManager.CurrentCheckpoint.transform;
        Vector3 direction = (checkpoint.position - player.position).normalized;
        Vector3 midpoint = Vector3.Lerp(player.position, checkpoint.position, 0.5f);
        Vector3 sideOffset = Vector3.Cross(direction, Vector3.up) * Random.Range(-20f, 20f);
        Vector3 heightOffset = Vector3.up * Random.Range(5f, 15f);

        return midpoint + sideOffset + heightOffset;
    }

    public void CrowFinished()
    {
        activeCrow = null;
    }
}
