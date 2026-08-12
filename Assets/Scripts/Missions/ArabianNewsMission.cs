using UnityEngine;

[CreateAssetMenu(fileName = "ArabianNewsMission", menuName = "Game/Missions/Arabian News Mission")]
public class ArabianNewsMission : MissionData
{
    public int minClients = 3;
    public int maxClients = 4;
    public float timerMultiplier = 1f;
    public float throttleMultiplier = 1f;
}
