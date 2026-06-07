using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public GameData Data { get; private set; }

    public int TotalCoins => Data.totalCoins;

    public int BestScore => Data.bestScore;

    public int TotalRuns => Data.totalRuns;

    public string SelectedSkin => Data.selectedSkin;

    private static string SAVE_PATH => Path.Combine(Application.persistentDataPath, "gamedata.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(SAVE_PATH, json);
    }

    public void Load()
    {
        if (!File.Exists(SAVE_PATH))
        {
            Data = new GameData();
            Save();
            return;
        }
        string json = File.ReadAllText(SAVE_PATH);
        Data = JsonUtility.FromJson<GameData>(json);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;
        Data.totalCoins += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0)
            return false;
        if (Data.totalCoins < amount)
            return false;
        Data.totalCoins -= amount;
        Save();
        return true;
    }

    public void TrySetBestScore(int score)
    {
        if (score <= Data.bestScore)
            return;

        Data.bestScore = score;
    }

    public void SaveRunResults(int score, int coins)
    {
        AddCoins(coins);
        TrySetBestScore(score);
        Save();
    }

    public void AddRun()
    {
        Data.totalRuns++;
        Save();
    }

    public void UnlockSkin(string skinId)
    {
        if (Data.unlockedSkins.Contains(skinId))
            return;

        Data.unlockedSkins.Add(skinId);
        Save();
    }

    public bool IsSkinUnlocked(string skinId)
    {
        return Data.unlockedSkins.Contains(skinId);
    }

    public void SelectSkin(string skinId)
    {
        if (!IsSkinUnlocked(skinId))
            return;

        Data.selectedSkin = skinId;
        Save();
    }

    public void ResetSave()
    {
        Data = new GameData();
        Save();
    }
}
