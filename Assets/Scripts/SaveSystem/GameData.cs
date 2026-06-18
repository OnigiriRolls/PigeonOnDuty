using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int totalCoins;
    public int reputation;
    public string selectedSkin;
    public List<string> unlockedSkins = new();
    public List<string> unlockedContentIds = new();
    public int totalRuns;
}
