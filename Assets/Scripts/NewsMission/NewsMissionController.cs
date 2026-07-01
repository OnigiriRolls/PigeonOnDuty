using UnityEngine;

public class NewsMissionController : MonoBehaviour
{
    [SerializeField] private PedestrianSpawner pedestrianSpawner;

    public void StartMission()
    {
        pedestrianSpawner.SpawnPedestrians();
    }
}
