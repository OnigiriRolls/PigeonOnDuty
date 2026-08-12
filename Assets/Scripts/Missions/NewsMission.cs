using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewsMission", menuName = "Game/Missions/News Mission")]
public class NewsMission : MissionData
{
    public int minClients = 3;
    public int maxClients = 4;
}
