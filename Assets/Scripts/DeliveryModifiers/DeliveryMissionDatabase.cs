using System.Collections.Generic;
using UnityEngine;

public class DeliveryMissionDatabase : MonoBehaviour
{
    public static DeliveryMissionDatabase Instance;

    public List<DeliveryMission> Missions => missions;

    [SerializeField] private List<DeliveryMission> missions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
