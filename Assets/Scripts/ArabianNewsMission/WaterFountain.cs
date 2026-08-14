using UnityEngine;

public class WaterFountain : MonoBehaviour
{
    [SerializeField] private float hydrationPerSecond = 20f;
    [SerializeField] private MinimapMissionController minimapController;

    private PlayerHydration playerHydration;

    private void Start()
    {
        minimapController.ShowWell(transform);
    }

    private void Update()
    {
        if (playerHydration == null)
            return;
        playerHydration.RestoreHydration(hydrationPerSecond * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        PlayerHydration hydration = other.GetComponentInParent<PlayerHydration>();
        playerHydration = hydration;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        playerHydration = null;
    }

    private void OnDestroy()
    {
        if (minimapController != null)
            minimapController.HideWell(transform);
    }
}
