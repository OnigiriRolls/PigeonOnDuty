using UnityEngine;

[CreateAssetMenu(fileName = "PigeonSkinData", menuName = "Game/Shop/Pigeon Skin")]
public class PigeonSkinData : ScriptableObject
{
    public string id;
    public string skinName;
    public Color color = Color.white;
    public int price;
}
