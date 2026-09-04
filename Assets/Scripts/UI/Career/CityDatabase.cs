using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityDatabase", menuName = "Game/Cities/City Database")]
public class CityDatabase : ScriptableObject
{
    public List<CityData> cities;
}
